output "fqdn" {
  value = "https://${azurerm_container_app.this.ingress[0].fqdn}"
}