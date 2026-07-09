# Infrastructure Runbook — SplatDev.Umbraco.Plugins

## Target OS

Ubuntu Server 24.04 LTS on Azure (marketplace image) or AWS (Canonical AMI).

## 1. Provision a New Server

### Azure (CLI)

```sh
az group create --name splatdev-plugins-rg --location eastus2
az vm create \
  --resource-group splatdev-plugins-rg \
  --name splatdev-plugins-vm \
  --image Ubuntu2204 \
  --admin-username splatdev \
  --generate-ssh-keys \
  --size Standard_B2s
az vm open-port --resource-group splatdev-plugins-rg --name splatdev-plugins-vm --port 8888
```

### AWS (CLI)

```sh
aws ec2 run-instances \
  --image-id ami-0e86e20dae9224db8 \
  --instance-type t3.small \
  --key-name splatdev-key \
  --security-group-ids sg-xxxxxxxx \
  --tag-specifications 'ResourceType=instance,Tags=[{Key=Name,Value=splatdev-plugins}]'
```

## 2. Base Server Hardening

```sh
sudo apt update && sudo apt upgrade -y
sudo apt install -y ufw fail2ban unattended-upgrades
sudo ufw allow 22/tcp
sudo ufw allow 80,443/tcp
sudo ufw allow 8888/tcp
sudo ufw enable
sudo systemctl enable --now fail2ban
```

## 3. Install aaPanel

```sh
wget -O install.sh http://www.aapanel.com/script/install-ubuntu_6.0.sh
sudo bash install.sh
```

Post-install: note the panel URL, username, and password from the installer output.

## 4. Harden aaPanel

- Change default aaPanel port (Settings → Panel Port).
- Enable HTTPS for the panel (SSL → Let's Encrypt).
- Enable 2FA (Settings → Two-Factor Authentication).
- Remove unused plugins and default apps.

## 5. Web Stack (via aaPanel)

- Install Nginx or Apache (Website → Add Site).
- Install MySQL / MariaDB (App Store → Database).
- Configure PHP versions if needed (App Store → PHP).

## 6. Firewall (via aaPanel)

- Security → Firewall: restrict aaPanel port to operator IP.
- Expose only ports: 22, 80, 443.

## 7. Backups (via aaPanel)

- Scheduled tasks → Backup: daily DB + site files.
- Target: local disk or cloud storage (S3 / Azure Blob).

## 8. Secrets

Secrets are **never stored in this repo**. Reference by key name only:
`$PANEL_PASSWORD`, `$PROD_HOST`, `$AZURE_TENANT`, `$AWS_ACCESS_KEY`.

## 9. Recovery

1. Provision new server (Section 1).
2. Harden and install aaPanel (Sections 2–4).
3. Restore from backup (Section 7).
4. Verify DNS and SSL.
5. Smoke test: `curl -I https://<host>/umbraco`.
