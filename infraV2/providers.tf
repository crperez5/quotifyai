terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.106.1"
    }

    azuread = {
      source  = "hashicorp/azuread"
      version = "2.50.0"
    }

    github = {
      source  = "integrations/github"
      version = "6.2.1"
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