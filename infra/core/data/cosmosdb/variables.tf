variable "resource_group_name" {
  type        = string
  description = "Nombre del resource group donde se creará Cosmos DB"
}

variable "environment" {
  type = string
}

variable "location" {
  type        = string
  description = "Ubicación de Azure donde se desplegará el recurso"
}

variable "cosmos_db_account_name" {
  type        = string
  description = "Nombre de la cuenta de Cosmos DB"
}

variable "managed_identity_id" {
  type = string
}

variable "managed_principal_id" {
  type = string
}

variable "database_name" {
  type = string
}

variable "table_name" {
  type = string
}

variable "tags" {
  type        = map(string)
  description = "Tags para los recursos"
  default     = {}
}

variable "partition_key_paths" {
  type    = list(string)
  default = ["/userId"]
}

variable "composite_indexes" {
  type = list(object({
    path  = string
    order = string
  }))
  default = [
    {
      path  = "/userId"
      order = "ascending"
    },
    {
      path  = "/timestamp"
      order = "descending"
    }
  ]
}

