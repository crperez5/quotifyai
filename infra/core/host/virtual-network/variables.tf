variable "resource_group_name" {
  description = "Name of the resource group"
}

variable "location" {
  description = "Location of the resource"
}

variable "environment" {
  description = "Environment of the resource"
}

variable "vnet_name" {
  description = "Name of the vnet"
}

variable "subnet_name" {
  description = "Name of the subnet"
}

variable "address_space" {
  description = "Vnet address space"
}
 
variable "subnet_address_prefix_map" {
  type = map(list(string))
}

variable "tags" {
  description = "List of tags"
  type        = map(string)
}