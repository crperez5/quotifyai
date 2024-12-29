output cognitive_service_id {
    value = azurerm_cognitive_account.openai.id
}

output cognitive_service_endpoint {
    value = "https://${azurerm_cognitive_account.openai.custom_subdomain_name}.privatelink.openai.azure.com/"
}

output cognitive_service_role {
    value = "Cognitive Services OpenAI User"
}