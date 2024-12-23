variable "resource_group_name" {
  type        = string
  description = "Name of the resource group"
}

variable "location" {
  type        = string
  description = "Azure region location"
}

variable "key_vault_name" {
  type        = string
  description = "Name of the key vault"
}

variable "allowed_ips" {
  description = "List of IP addresses allowed to access the Key Vault"
  type        = list(string)
  default     = []
}
