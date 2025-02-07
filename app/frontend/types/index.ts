export interface Message {
  id: string
  content: string
  role: "user" | "assistant"
  createdAt: Date
}

export interface Conversation {
  id: string
  title: string
  messages: Message[]
}

export interface Material {
  id: string
  name: string
  pricePerSqm: number // This is now in euros
}

export interface Document {
  id: string
  name: string
  url: string
  uploadDate: Date
}

