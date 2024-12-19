terraform {
  required_version = ">= 1.1.7, < 2.0.0"
  backend "azurerm" {
  }
}  

provider "azurerm" {
  features {}
}