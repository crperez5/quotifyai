"use client"

import { useState, useEffect, useCallback } from "react"
import { api } from "../../services/api"
import type { Conversation, Message } from "../../types"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import type React from "react" // Added import for React

interface ChatInterfaceProps {
  activeConversation: Conversation
  setActiveConversation: React.Dispatch<React.SetStateAction<Conversation | null>>
}

export default function ChatInterface({ activeConversation, setActiveConversation }: ChatInterfaceProps) {
  const [input, setInput] = useState("")

  useEffect(() => {
    const unsubscribe = api.subscribeToMessages(({ conversationId, message }) => {
      if (activeConversation && activeConversation.id === conversationId) {
        setActiveConversation((prev) => ({
          ...prev,
          messages: [...prev.messages, message],
        }))
      }
    })

    return () => {
      unsubscribe()
    }
  }, [activeConversation, setActiveConversation])

  const handleSendMessage = useCallback(async () => {
    if (!input.trim()) return

    const userMessage = await api.addMessage(activeConversation.id, input, "user")
    setActiveConversation((prev) => ({
      ...prev,
      messages: [...prev.messages, userMessage],
    }))
    
    setInput("")
  }, [activeConversation.id, input, setActiveConversation])

  return (
    <div className="h-full flex flex-col">
      <ScrollArea className="flex-1 mb-4">
        {activeConversation.messages.map((message: Message) => (
          <Card key={message.id} className={`mb-4 ${message.role === "user" ? "bg-blue-100" : "bg-green-100"}`}>
            <CardContent className="p-4">
              <p className="font-bold">{message.role === "user" ? "You" : "AI"}</p>
              <p>{message.content}</p>
            </CardContent>
          </Card>
        ))}
      </ScrollArea>
      <div className="flex space-x-2">
        <Input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type your message..."
          onKeyPress={(e) => e.key === "Enter" && handleSendMessage()}
        />
        <Button onClick={handleSendMessage}>Send</Button>
      </div>
    </div>
  )
}

