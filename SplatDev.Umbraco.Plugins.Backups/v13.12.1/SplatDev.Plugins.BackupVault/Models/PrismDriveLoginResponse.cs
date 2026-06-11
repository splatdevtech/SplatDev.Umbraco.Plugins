using System.Text.Json.Serialization;

namespace SplatDev.Plugins.BackupVault.Models
{
    public class PrismDriveLoginResponse
    {
        [JsonPropertyName("themes")]
        public Themes Themes { get; set; } = new();
        [JsonPropertyName("user")]
        public User User { get; set; } = new();
        [JsonPropertyName("menus")]
        public object[]? Menus { get; set; }
        [JsonPropertyName("settings")]
        public Settings? Settings { get; set; }
        [JsonPropertyName("locales")]
        public Locale[]? Locales { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

    }

    public class Themes
    {
        [JsonPropertyName("light")]
        public Theme? Light { get; set; }
        [JsonPropertyName("dark")]
        public Theme? Dark { get; set; }
    }

    public class Theme
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("is_dark")]
        public bool IsDark { get; set; }
        [JsonPropertyName("default_light")]
        public bool DefaultLight { get; set; }
        [JsonPropertyName("default_dark")]
        public bool DefaultDark { get; set; }
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
        [JsonPropertyName("colors")]
        public Colors? Colors { get; set; }
        [JsonPropertyName("created_ad")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class Colors
    {
        [JsonPropertyName("beforegroundbase")]
        public int[]? BeForegroundBase { get; set; }
        [JsonPropertyName("beprimarylight")]
        public int[]? BePrimaryLight { get; set; }
        [JsonPropertyName("beprimary")]
        public int[]? BePrimary { get; set; }
        [JsonPropertyName("beprimarydark")]
        public int[]? BePrimaryDark { get; set; }
        [JsonPropertyName("beonprimary")]
        public int[]? BeOnPrimary { get; set; }
        [JsonPropertyName("bebackground")]
        public int[]? BeBackground { get; set; }
        [JsonPropertyName("bebackgroundalt")]
        public int[]? BeBackgroundAlt { get; set; }
        [JsonPropertyName("bebackgroundchip")]
        public int[]? BeBackgroundChip { get; set; }
        [JsonPropertyName("bepaper")]
        public int[]? BePaper { get; set; }
        [JsonPropertyName("bedisabledbgopacity")]
        public int BeDisabledBgOpacity { get; set; }
        [JsonPropertyName("bedisabledfgopacity")]
        public int BeDisabledFgOpacity { get; set; }
        [JsonPropertyName("behoveropacity")]
        public int BeHoverOpacity { get; set; }
        [JsonPropertyName("befocusopacity")]
        public int BeFocusOpacity { get; set; }
        [JsonPropertyName("beselectedopacity")]
        public int BeSelectedOpacity { get; set; }
        [JsonPropertyName("betextmainopacity")]
        public int BeTextMainOpacity { get; set; }
        [JsonPropertyName("betextmutedopacity")]
        public int BeTextMutedOpacity { get; set; }
        [JsonPropertyName("bedivideropacity")]
        public int BeDividerOpacity { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("two_factor_secret")]
        public object? TwoFactorSecret { get; set; }
        [JsonPropertyName("two_factor_recovery_codes")]
        public object? TwoFactorRecoveryCodes { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("space_available")]
        public object? SpaceAvailable { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = string.Empty;
        [JsonPropertyName("stripe_id")]
        public object? StripeId { get; set; }
        [JsonPropertyName("available_space")]
        public object? AvailableSpace { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("card_brand")]
        public object? CardBrand { get; set; }
        [JsonPropertyName("card_last_four")]
        public object? CardLastFour { get; set; }
        [JsonPropertyName("email_verified_at")]
        public DateTime EmailVerifiedAt { get; set; }
        [JsonPropertyName("card_expires")]
        public object? CardExpires { get; set; }
        [JsonPropertyName("paypal_id")]
        public object? PaypalId { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("fcm_token")]
        public object? FcmToken { get; set; }
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("has_password")]
        public bool HasPassword { get; set; }
        [JsonPropertyName("model_type")]
        public string ModelType { get; set; } = string.Empty;
    }

    public class Settings
    {
        [JsonPropertyName("socialgoogleenable")]
        public bool SocialGoogleEnable { get; set; }
    }

    public class Locale
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;
    }

}
