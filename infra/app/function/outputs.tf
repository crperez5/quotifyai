output "id" {
  value = azurerm_windows_function_app.this.identity[0].principal_id
}