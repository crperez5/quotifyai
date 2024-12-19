variable "oidc_token" {}
variable "oidc_token_file_path" {}
variable "oidc_request_token" {}
variable "oidc_request_url" {}

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
  use_oidc        = true
  oidc_request_token = var.oidc_request_token
  oidc_request_url   = var.oidc_request_url
  oidc_token = var.oidc_token  
  oidc_token_file_path = var.oidc_token_file_path

  features {}
}

provider "github" {
  token = var.github_token
  owner = var.github_organization_name
}