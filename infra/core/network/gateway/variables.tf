variable "gateway_name" {
  type = string
}

variable "environment" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "vnet_name" {
  type = string
}

variable "location" {
  type    = string
  default = "westeurope"
}

variable "backend_fqdn" {
  type = string
}

variable "storage_account_id" {
  type = string
}

variable "gateway_user_assigned_id" {
  type = string
}

# variable "ssl_certificate_name" {
#   type = string
# }

# variable "ssl_certificate_secret_id" {
#   type = string
# }

# variable "gateway_keyvault_access_policy_id" {
#   type = string
# }

variable "tags" {
  description = "List of tags"
  type        = map(string)
}