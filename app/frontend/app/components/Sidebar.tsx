"use client"
import { useState } from "react"
import { Button } from "@/components/ui/button"
import { ScrollArea } from "@/components/ui/scroll-area"
import { MessageSquare, Package, FileText, X } from "lucide-react"
import type { Conversation } from "../../types"

interface SidebarProps {
  setActiveView: (view: "chat" | "materials" | "documents") => void
  conversations: Conversation[]
  setActiveConversation: (conversation: Conversation) => void
  onNewConversation: () => void
  onDeleteConversation: (conversationId: string) => void
}

export default function Sidebar({
  setActiveView,
  conversations,
  setActiveConversation,
  onNewConversation,
  onDeleteConversation
}: SidebarProps) {
  const [hoveredConversation, setHoveredConversation] = useState<string | null>(null)

  return (
    <div className="w-64 bg-gray-800 text-white p-4 flex flex-col">
      <Button className="mb-4 bg-navy-light text-white hover:bg-navy-dark" onClick={onNewConversation}>
        New Chat
      </Button>
      <nav className="flex flex-col space-y-2 flex-grow overflow-hidden">
        <Button variant="ghost" className="w-full justify-start" onClick={() => setActiveView("chat")}>
          <MessageSquare className="mr-2 h-4 w-4" /> Chats
        </Button>
        <ScrollArea className="flex-grow">
          {conversations.map((conversation) => (
            <div
              key={conversation.id}
              className="relative group"
              onMouseEnter={() => setHoveredConversation(conversation.id)}
              onMouseLeave={() => setHoveredConversation(null)}
            >
              <Button
                variant="ghost"
                className="w-full justify-start pl-8 text-sm"
                onClick={() => setActiveConversation(conversation)}
              >
                {conversation.title}
              </Button>
              {hoveredConversation === conversation.id && (
                <button
                  className="absolute right-2 top-1/2 transform -translate-y-1/2 p-1 bg-transparent hover:bg-transparent focus:outline-none"
                  onClick={(e) => {
                    e.stopPropagation()
                    onDeleteConversation(conversation.id)
                  }}
                >
                  <X className="h-4 w-4 text-white" />
                </button>
              )}
            </div>
          ))}
        </ScrollArea>
        <Button variant="ghost" className="w-full justify-start" onClick={() => setActiveView("materials")}>
          <Package className="mr-2 h-4 w-4" /> Materials
        </Button>
        <Button variant="ghost" className="w-full justify-start" onClick={() => setActiveView("documents")}>
          <FileText className="mr-2 h-4 w-4" /> Documents
        </Button>
      </nav>
    </div>
  )
}

