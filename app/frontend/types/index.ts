export interface Message {
  id: string
  content: string
  role: "user" | "assistant"
  createdAt?: string
}

export interface Conversation {
  id: string
  title: string
  createdAt?: string
  lastMessageAt?: string  
  messages: Message[]
}

export interface Material {
  id: string
  name: string
  pricePerSqm: number 
  createdAt?: string  
}

export interface Document {
  id: string
  name: string
  url: string
  createdAt?: string
}