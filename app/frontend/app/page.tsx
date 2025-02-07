"use client"

import { useState, useEffect, useCallback } from "react"
import Sidebar from "./components/Sidebar"
import ChatInterface from "./components/ChatInterface"
import MaterialList from "./components/MaterialList"
import DocumentList from "./components/DocumentList"
import type { Conversation } from "../types"
import { api } from "../services/api"
import { ConversationService } from "../services/ConversationService"

export default function Home() {
  const [activeView, setActiveView] = useState<"chat" | "materials" | "documents">("chat")
  const [conversations, setConversations] = useState<Conversation[]>([])
  const [activeConversation, setActiveConversation] = useState<Conversation | null>(null)

  useEffect(() => {
    loadConversations()
  }, [])

  const loadConversations = async () => {
    const loadedConversations = await api.getConversations()
    setConversations(loadedConversations)
  }

  const handleNewConversation = useCallback(async () => {
    const newConversation = await ConversationService.create()
    setConversations((prev) => [...prev, newConversation])
    setActiveConversation(newConversation)
    setActiveView("chat")
  }, [])

  const handleSetActiveConversation = useCallback((conversation: Conversation) => {
    setActiveConversation(conversation)
    setActiveView("chat")
  }, [])

  return (
    <div className="flex h-screen bg-gray-100">
      <Sidebar
        setActiveView={setActiveView}
        conversations={conversations}
        setActiveConversation={handleSetActiveConversation}
        onNewConversation={handleNewConversation}
      />
      <main className="flex-1 overflow-y-auto p-4">
        {activeView === "chat" && activeConversation ? (
          <ChatInterface activeConversation={activeConversation} setActiveConversation={setActiveConversation} />
        ) : activeView === "chat" ? (
          <div className="flex h-full items-center justify-center">
            <p className="text-xl text-gray-600">Do you need to calculate a quote today? Start a new conversation.</p>
          </div>
        ) : activeView === "materials" ? (
          <MaterialList />
        ) : (
          <DocumentList />
        )}
      </main>
    </div>
  )
}

