variable "environment" {
  type = string
}
variable "app_name" {
  description = "App name"
  type        = string
}

variable "app_id" {
  description = "App id"
  type        = string
  default     = null
}

variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "image_name" {
  description = "Container image"
  type        = string
}

variable "container_registry_url" {
  description = "Container registry url"
  type        = string
  default     = null
}

variable "user_identity_id" {
  description = "The Identity ID to use"
  type        = string
  default     = null
}

variable "tags" {
  description = "List of tags"
  type        = map(string)
}

variable "env" {
  description = "List of env variables"
  type = list(object({
    name  = string
    value = string
  }))
  default = []
}

variable "volume_mount_name" {
  type    = string
  default = null
}

variable "volume_mount_path" {
  type    = string
  default = null
}

variable "file_share_name" {
  type    = string
  default = null
}

variable "ingress_target_port" {
  type    = number
  default = 80
}

variable "ingress_external_enabled" {
  type    = bool
  default = true
}

variable "ingress_allow_insecure_connections" {
  type    = bool
  default = false
}

variable "ingress_transport" {
  type    = string
  default = null
}

variable "container_apps_environment_name" {
  type = string
}

variable "liveness_probe" {
  description = "Liveness probe configuration"
  type = object({
    port      = number
    transport = string
    path      = string
  })
  default = null
}

variable "readiness_probe" {
  description = "Readiness probe configuration"
  type = object({
    port      = number
    transport = string
    path      = string
  })
  default = null
}