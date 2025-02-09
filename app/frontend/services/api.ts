import type { Conversation, Material, Message, Document } from "../types"
import { EventEmitter } from "events"

// Mock data
let conversations: Conversation[] = []
let materials: Material[] = []
let documents: Document[] = []

const eventEmitter = new EventEmitter()

export const api = {
  // Conversation methods
  getConversations: async (): Promise<Conversation[]> => {
    return conversations
  },

  createConversation: async (title: string): Promise<Conversation> => {
    const newConversation: Conversation = {
      id: Date.now().toString(),
      title,
      messages: [],
    }
    conversations = [...conversations, newConversation]

    // Simulate a POST request
    await new Promise((resolve) => setTimeout(resolve, 300))

    return newConversation
  },

  addMessage: async (conversationId: string, content: string, role: "user" | "assistant"): Promise<Message> => {
    const conversation = conversations.find((c) => c.id === conversationId)
    if (!conversation) throw new Error("Conversation not found")

    const newMessage: Message = {
      id: Date.now().toString(),
      content,
      role,
      createdAt: new Date(),
    }
    conversation.messages = [...conversation.messages, newMessage]

    // Simulate a POST request
    await new Promise((resolve) => setTimeout(resolve, 300))

    // If it's a user message, simulate an AI response via WebSocket
    if (role === "user") {
      setTimeout(() => {
        const aiMessage: Message = {
          id: (Date.now() + 1).toString(),
          content: "This is a mock AI response via WebSocket.",
          role: "assistant",
          createdAt: new Date(),
        }
        conversation.messages = [...conversation.messages, aiMessage]
        eventEmitter.emit("newMessage", { conversationId, message: aiMessage })
      }, 1000)
    }

    return newMessage
  },

  // Material methods
  getMaterials: async (): Promise<Material[]> => {
    return materials
  },

  createMaterial: async (name: string, pricePerSqm: number): Promise<Material> => {
    const newMaterial: Material = {
      id: Date.now().toString(),
      name,
      pricePerSqm,
    }
    materials.push(newMaterial)
    return newMaterial
  },

  updateMaterial: async (id: string, name: string, pricePerSqm: number): Promise<Material> => {
    const index = materials.findIndex((m) => m.id === id)
    if (index === -1) throw new Error("Material not found")

    materials[index] = { ...materials[index], name, pricePerSqm }
    return materials[index]
  },

  deleteMaterial: async (id: string): Promise<void> => {
    materials = materials.filter((m) => m.id !== id)
  },

  // Document methods
  getDocuments: async (): Promise<Document[]> => {
    return documents
  },

  uploadDocument: async (file: File): Promise<Document> => {
    const newDocument: Document = {
      id: Date.now().toString(),
      name: file.name,
      url: URL.createObjectURL(file), // In a real app, this would be a server-side URL
      createdAt: new Date(),
    }

    await new Promise((resolve) => setTimeout(resolve, 1000))

    return newDocument
  },

  deleteDocument: async (id: string): Promise<void> => {
    await new Promise((resolve) => setTimeout(resolve, 300))
  },

  // WebSocket simulation
  subscribeToMessages: (callback: (data: { conversationId: string; message: Message }) => void) => {
    eventEmitter.on("newMessage", callback)
    return () => {
      eventEmitter.off("newMessage", callback)
    }
  },
}

