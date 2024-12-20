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

    github = {
      source  = "integrations/github"
      version = "6.4.0"
    }
  }
}

provider "azurerm" {
  subscription_id = var.azure_subscription_id

  features {}
}

provider "github" {
  token = var.github_token
  owner = var.github_organization_name
}