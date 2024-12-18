variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe" 
}
