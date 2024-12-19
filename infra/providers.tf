terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }

    azuread = {
      source  = "hashicorp/azuread"
      version = "2.50.0"
    }    
  }

  backend "azurerm" {
    use_oidc = true   
    resource_group_name = "tfstate"
    storage_account_name = "tfstatequotifyai" 
    container_name = "tfstate"
    key = "azd/azdremotetest.tfstate"
  }  
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
  use_oidc        = true

}