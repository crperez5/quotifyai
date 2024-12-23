variable "key_vault_id" {
  type        = string
  description = "ID of the key vault"
}

variable "managed_identity_object_id" {
  type        = string
  description = "Object ID of the managed identity that needs access to the key vault"
}

variable "enable_set_permission" {
  description = "Flag to enable the 'set' permission on the key vault secret"
  type        = bool
  default     = false
}