terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.14.0"
    }

    azuread = {
      source  = "hashicorp/azuread"
      version = "3.0.2"
    }
  }

  backend "azurerm" {
    use_oidc             = true
    resource_group_name  = "tfstate"
    storage_account_name = "tfstatequotifyai"
    container_name       = "tfstate"
    key                  = "azd/azdremotetest.tfstate"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
  subscription_id = var.azure_subscription_id
  use_oidc        = true

}
