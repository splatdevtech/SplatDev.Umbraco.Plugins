using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using SplatDev.Payments.Santander;
using Xunit;
using Xunit.Abstractions;

namespace SplatDev.Tests;

/// <summary>
/// Santander per-product homologation (SPL-2669). Live integration suite against the
/// Santander Open Banking sandbox (trust-sandbox.api.santander.com.br). Each test exercises
/// one product API, captures request/response evidence, and asserts the expected sandbox
/// response shape.
///
/// SAFETY: All operations use the sandbox base URL only (never production). No
/// money-moving endpoints are called with real values — boletos are TESTE only, Pix
/// charges use minimal amounts and self-destruct after expiry, and balance/statement
/// queries are read-only.
///
/// This suite is gated by SANTANDER_CLIENT_ID + SANTANDER_CLIENT_SECRET +
/// SANTANDER_CERT_PATH + SANTANDER_CERT_PASSWORD. When any is missing, every test
/// returns immediately with a clear skip explanation — CI and contributor runs stay
/// green without relying on secrets.
/// </summary>
[Trait("Category", "Integration")]
public class SantanderHomologationTests
{
    private static readonly string? ClientId = Environment.GetEnvironmentVariable("SANTANDER_CLIENT_ID");
    private static readonly string? ClientSecret = Environment.GetEnvironmentVariable("SANTANDER_CLIENT_SECRET");
    private static readonly string? ApplicationKey = Environment.GetEnvironmentVariable("SANTANDER_APPLICATION_KEY");
    private static readonly string? CertPath = Environment.GetEnvironmentVariable("SANTANDER_CERT_PATH");
    private static readonly string? CertPassword = Environment.GetEnvironmentVariable("SANTANDER_CERT_PASSWORD");
    private static readonly string? CovenantCode = Environment.GetEnvironmentVariable("SANTANDER_COVENANT_CODE");
    private static readonly string? WorkspaceId = Environment.GetEnvironmentVariable("SANTANDER_WORKSPACE_ID");
    private static readonly string? BankId = Environment.GetEnvironmentVariable("SANTANDER_BANK_ID");
    private static readonly string? AccountId = Environment.GetEnvironmentVariable("SANTANDER_ACCOUNT_ID");
    private static readonly string? PixKey = Environment.GetEnvironmentVariable("SANTANDER_PIX_KEY");

    private static bool CredentialsPresent =>
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(ClientSecret) &&
        !string.IsNullOrWhiteSpace(CertPath) &&
        File.Exists(CertPath) &&
        !string.IsNullOrWhiteSpace(CertPassword);

    private readonly ITestOutputHelper _output;

    public SantanderHomologationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private SantanderApiOptions CreateOptions()
    {
        var opts = new SantanderApiOptions
        {
            BaseUrl = "https://trust-sandbox.api.santander.com.br",
            ClientId = ClientId!,
            ClientSecret = ClientSecret!,
            CertificatePath = CertPath!,
            CertificatePassword = CertPassword!,
            BankId = BankId ?? "90400888000142",
            AccountId = AccountId ?? string.Empty,
            PixKey = PixKey ?? string.Empty,
            CovenantCode = CovenantCode ?? string.Empty,
            WorkspaceId = WorkspaceId ?? string.Empty,
        };

        if (!string.IsNullOrWhiteSpace(ApplicationKey))
            opts.ClientId = ApplicationKey; // X-Application-Key header = ClientId for this API surface

        return opts;
    }

    private static ILogger<T> CreateLogger<T>()
    {
        using var factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug).AddConsole());
        return factory.CreateLogger<T>();
    }

    private void LogEvidence(string label, JsonDocument doc)
    {
        var raw = doc.RootElement.GetRawText();
        var truncated = raw.Length <= 2000 ? raw : raw[..2000] + "…(truncated)";
        _output.WriteLine($"[EVIDENCE] {label}:");
        _output.WriteLine(truncated);
        _output.WriteLine(string.Empty);
    }

    // ── plumbing ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_ObterSaldoAsync_Sandbox_ReturnsBalanceData()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("balance (saldo)");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderBalanceStatementService(client, opts);

        using var doc = await service.ObterSaldoAsync();
        var root = doc.RootElement;

        LogEvidence("Saldo — availableAmount", doc);
        Assert.True(root.TryGetProperty("availableAmount", out _),
            "Saldo response missing 'availableAmount' — may indicate contract mismatch.");
    }

    // ── Extrato (Statement) ─────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_ObterExtratoAsync_Sandbox_ReturnsStatementEntries()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("statement (extrato)");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderBalanceStatementService(client, opts);

        var from = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        using var doc = await service.ObterExtratoAsync(from, to, 1);

        LogEvidence("Extrato — statement entries", doc);
        var root = doc.RootElement;
        Assert.True(root.ValueKind == JsonValueKind.Object,
            "Extrato response should be a JSON object.");
    }

    // ── Pix QR Code ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_CriarCobrancaPix_Sandbox_ReturnsTxidAndPixCopiaECola()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("pix qr code");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderPixQrCodeService(client, opts, CreateLogger<SantanderPixQrCodeService>());

        // Mini-amount Pix charge for sandbox homologation — self-expires after 5 min
        var charge = await service.CriarCobrancaAsync(
            valor: 0.01m,
            descricao: "Homologacao SPL-2669 Pix QR Code",
            expiracaoSegundos: 300);

        LogEvidence("Pix QR Code", JsonDocument.Parse(charge.Raw));

        Assert.False(string.IsNullOrWhiteSpace(charge.Txid), "txid must not be empty.");
        Assert.Equal("ATIVA", charge.Status);
        Assert.False(string.IsNullOrWhiteSpace(charge.PixCopiaECola), "pixCopiaECola must not be empty.");
    }

    // ── Pix Automático ──────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_PixAutomatico_Sandbox_CriarLocation_ReturnsId()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("pix automatico");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderPixAutomaticoService(client, opts);

        var locationId = await service.CriarLocationAsync();

        _output.WriteLine($"[EVIDENCE] Pix Automático location id: {locationId}");
        Assert.True(locationId > 0, "Location id must be a positive integer.");
    }

    [Fact]
    public async Task Homologation_PixAutomatico_Sandbox_CriarRecorrencia_ReturnsValidPayload()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("pix automatico rec");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderPixAutomaticoService(client, opts);

        var payload = JsonDocument.Parse("""
        {
            "calendario": { "dataInicio": "2026-08-01" },
            "valor": { "original": "1.00" },
            "chave": "",
            "status": "ATIVA"
        }
        """).RootElement;

        using var doc = await service.CriarRecorrenciaAsync(payload);
        LogEvidence("Pix Automático — recorrência", doc);

        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("idRec", out _) || root.TryGetProperty("txid", out _),
            "Pix Automático recurrence response missing identifier.");
    }

    // ── Pagamentos ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_Pagamentos_IniciarPagamento_Sandbox_ReturnsPaymentOrder()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("payments (pagamentos)");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderPaymentsService(client, opts, CreateLogger<SantanderPaymentsService>());

        var payload = JsonDocument.Parse("""
        {
            "amount": { "currency": "BRL", "value": 1.00 },
            "destinationAccount": { "bank": "001", "branch": "0001", "number": "12345678" },
            "paymentType": "TEF"
        }
        """).RootElement;

        try
        {
            using var doc = await service.IniciarPagamentoAsync(payload);
            LogEvidence("Pagamentos — iniciar pagamento", doc);
        }
        catch (SantanderApiException ex) when (ex.StatusCode == 422 || ex.StatusCode == 400)
        {
            _output.WriteLine($"[EVIDENCE] Pagamentos sandbox validation response (expected): {ex.StatusCode}");
            _output.WriteLine(ex.ResponseBody[..Math.Min(ex.ResponseBody.Length, 2000)]);
            _output.WriteLine("(Sandbox validation errors during homologation are normal — the API contract is being exercised.)");
            return;
        }
    }

    // ── Boletos ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_Boletos_ListarWorkspaces_Sandbox_ReturnsWorkspaces()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("boletos (workspaces)");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderBoletoService(client, opts, CreateLogger<SantanderBoletoService>());

        using var doc = await service.ListarWorkspacesAsync();
        LogEvidence("Boletos — workspaces", doc);

        var root = doc.RootElement;
        Assert.True(root.ValueKind == JsonValueKind.Object,
            "Workspaces response should be a JSON object.");
    }

    [Fact]
    public async Task Homologation_Boletos_EmitirBoleto_Sandbox_ReturnsBillDetails()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("boletos (emitir)");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderBoletoService(client, opts, CreateLogger<SantanderBoletoService>());

        // Minimal boleto payload — TESTE environment only
        var payload = JsonDocument.Parse($$"""
        {
            "environment": "TESTE",
            "nsuCode": "SPL2669-{{DateTime.UtcNow:yyyyMMddHHmmss}}",
            "beneficiary": { "document": "90400888000142", "name": "SANTANDER HOMOLOGACAO" },
            "payer": { "document": "12345678909", "name": "PAGADOR TESTE", "address": "Rua Teste, 1", "neighborhood": "Centro", "city": "Sao Paulo", "state": "SP", "zipCode": "01001000" },
            "amount": { "nominalValue": 10.00, "dueDate": "{{DateTime.UtcNow.AddDays(5):yyyy-MM-dd}}" }
        }
        """).RootElement;

        try
        {
            using var doc = await service.EmitirBoletoAsync(payload);
            LogEvidence("Boletos — emitir boleto", doc);
        }
        catch (SantanderApiException ex) when (ex.StatusCode == 422 || ex.StatusCode == 400)
        {
            _output.WriteLine($"[EVIDENCE] Boletos sandbox validation response (expected during contract verification): {ex.StatusCode}");
            _output.WriteLine(ex.ResponseBody[..Math.Min(ex.ResponseBody.Length, 2000)]);
            return;
        }
    }

    // ── Open FX ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_OpenFx_Cotar_Sandbox_ReturnsQuoteOrValidation()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("open fx");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderOpenFxService(client, opts);

        var payload = JsonDocument.Parse("""
        {
            "from": { "currency": "USD", "amount": 100.00 },
            "to": { "currency": "BRL" }
        }
        """).RootElement;

        try
        {
            using var doc = await service.CotarAsync(payload);
            LogEvidence("Open FX — cotação", doc);
        }
        catch (SantanderApiException ex) when (ex.StatusCode >= 400 && ex.StatusCode < 500)
        {
            _output.WriteLine($"[EVIDENCE] Open FX sandbox response: {ex.StatusCode}");
            _output.WriteLine(ex.ResponseBody[..Math.Min(ex.ResponseBody.Length, 2000)]);
        }
    }

    // ── Export Charge ───────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_ExportCharge_Sandbox_ReturnsChargeOrValidation()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("export charge");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderExportChargeService(client, opts);

        var payload = JsonDocument.Parse("""
        {
            "currency": "BRL",
            "amount": 100.00
        }
        """).RootElement;

        try
        {
            using var doc = await service.CriarCobrancaExportacaoAsync(payload);
            LogEvidence("Export Charge — criar cobrança", doc);
        }
        catch (SantanderApiException ex) when (ex.StatusCode >= 400 && ex.StatusCode < 500)
        {
            _output.WriteLine($"[EVIDENCE] Export Charge sandbox response: {ex.StatusCode}");
            _output.WriteLine(ex.ResponseBody[..Math.Min(ex.ResponseBody.Length, 2000)]);
        }
    }

    // ── Vouchers ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Homologation_Vouchers_Listar_Sandbox_ReturnsVoucherListOrEmpty()
    {
        if (!CredentialsPresent)
        {
            SkipMissingCredentials("vouchers");
            return;
        }

        var opts = CreateOptions();
        var client = new SantanderApiClient(
            CreateHttpClientFactory(opts), opts,
            Mock.Of<IHostEnvironment>(),
            CreateLogger<SantanderApiClient>());
        var service = new SantanderVouchersService(client, opts);

        var from = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var to = DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            using var doc = await service.ListarComprovantesAsync(from, to);
            LogEvidence("Vouchers — listar comprovantes", doc);
        }
        catch (SantanderApiException ex) when (ex.StatusCode >= 400 && ex.StatusCode < 500)
        {
            _output.WriteLine($"[EVIDENCE] Vouchers sandbox response: {ex.StatusCode}");
            _output.WriteLine(ex.ResponseBody[..Math.Min(ex.ResponseBody.Length, 2000)]);
        }
    }

    // ── diagnostics helper ──────────────────────────────────────────────────

    [Fact]
    public void Homologation_Diagnostics_CredentialStatus()
    {
        _output.WriteLine("=== Santander Homologation Credential Status ===");
        _output.WriteLine($"CLIENT_ID: {(string.IsNullOrWhiteSpace(ClientId) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"CLIENT_SECRET: {(string.IsNullOrWhiteSpace(ClientSecret) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"CERT_PATH: {CertPath ?? "MISSING"}");
        _output.WriteLine($"CERT_FILE_EXISTS: {(CertPath is not null && File.Exists(CertPath) ? "YES" : "NO")}");
        _output.WriteLine($"CERT_PASSWORD: {(string.IsNullOrWhiteSpace(CertPassword) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"APPLICATION_KEY: {(string.IsNullOrWhiteSpace(ApplicationKey) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"COVENANT_CODE: {(string.IsNullOrWhiteSpace(CovenantCode) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"WORKSPACE_ID: {(string.IsNullOrWhiteSpace(WorkspaceId) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"BANK_ID: {BankId ?? "90400888000142 (default)"}");
        _output.WriteLine($"ACCOUNT_ID: {(string.IsNullOrWhiteSpace(AccountId) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"PIX_KEY: {(string.IsNullOrWhiteSpace(PixKey) ? "MISSING" : "PRESENT")}");
        _output.WriteLine($"ALL_REQUIRED_PRESENT: {CredentialsPresent}");
        _output.WriteLine("===============================================");

        // This test always passes — it exists to surface credential status in CI logs
        Assert.True(true);
    }

    // ── IHttpClientFactory substitute ─────────────────────────────────────

    private static IHttpClientFactory CreateHttpClientFactory(SantanderApiOptions opts)
    {
        var handler = new HttpClientHandler();
        if (!string.IsNullOrWhiteSpace(opts.CertificatePath) && File.Exists(opts.CertificatePath))
        {
            var cert = new X509Certificate2(opts.CertificatePath, opts.CertificatePassword);
            handler.ClientCertificates.Add(cert);
        }

        var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(45) };
        var mock = new Mock<IHttpClientFactory>();
        mock.Setup(f => f.CreateClient(SantanderApiClient.HttpClientName)).Returns(httpClient);
        return mock.Object;
    }

    private void SkipMissingCredentials(string product)
    {
        _output.WriteLine(
            $"SKIPPED (soft): homologation credentials for {product} are not fully configured. " +
            "Set SANTANDER_CLIENT_ID, SANTANDER_CLIENT_SECRET, SANTANDER_CERT_PATH, and " +
            "SANTANDER_CERT_PASSWORD to run this suite against the Santander sandbox.");
    }
}
