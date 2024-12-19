variable "oidc_token" {}
variable "oidc_token_file_path" {}
variable "oidc_request_token" {}
variable "oidc_request_url" {}

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }
  }

  backend "azurerm" {
  }  
}

provider "azurerm" {
  features {}

  use_oidc        = true

  oidc_request_token = var.oidc_request_token
  oidc_request_url   = var.oidc_request_url

  oidc_token = var.oidc_token

  oidc_token_file_path = var.oidc_token_file_path
}