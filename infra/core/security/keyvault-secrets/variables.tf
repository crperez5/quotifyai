variable "resource_group_name" {
  type        = string
  description = "Name of the resource group"
}

variable "key_vault_name" {
  description = "Name of the existing Key Vault"
  type        = string
}

variable "secrets" {
  description = "List of secrets to be created"
  type = list(object({
    name        = string
    value       = string
    enabled     = optional(bool, true)
    exp         = optional(number, 0)
    nbf         = optional(number, 0)
    contentType = optional(string, "string")
  }))
  default = []
}