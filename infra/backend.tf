terraform {
  backend "local" {
    path = "./terraform.tfstate"
  }
}


# terraform {
#   backend "azurerm" {
#     key      = "terraform.tfstate"
#     use_oidc = true
#   }
# }


