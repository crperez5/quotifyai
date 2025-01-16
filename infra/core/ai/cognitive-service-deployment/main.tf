# resource "azurerm_cognitive_deployment" "chatgpt" {
#   name                 = "chatgpt-model-deployment"
#   cognitive_account_id = var.cognitive_account_id

#   model {
#     format  = "OpenAI"
#     name    = "gpt-4o-mini"
#     version = "2024-07-18"
#   }

#   sku {
#     name = "GlobalStandard"
#   }
# }

resource "azurerm_cognitive_deployment" "embeddings" {
  name                 = "embedding-model-deployment"
  cognitive_account_id = var.cognitive_account_id

  model {
    format  = "OpenAI"
    name    = "text-embedding-ada-002"
    version = "2"
  }

  sku {
    name = "Standard"
  }
}
