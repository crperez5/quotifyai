"use client"

import { Button } from "@/components/ui/button"
import { ScrollArea } from "@/components/ui/scroll-area"
import { MessageSquare, Package, FileText } from "lucide-react"
import type { Conversation } from "../../types"

interface SidebarProps {
  setActiveView: (view: "chat" | "materials" | "documents") => void
  conversations: Conversation[]
  setActiveConversation: (conversation: Conversation) => void
  onNewConversation: () => void
}

export default function Sidebar({
  setActiveView,
  conversations,
  setActiveConversation,
  onNewConversation,
}: SidebarProps) {
  return (
    <div className="w-64 bg-gray-800 text-white p-4 flex flex-col">
      <Button
        className="mb-4 bg-navy-light text-white hover:bg-navy-dark"
        onClick={() => {
          console.log("New Chat button clicked")
          onNewConversation()
        }}
      >
        New Chat
      </Button>
      <nav className="flex flex-col space-y-2 flex-grow overflow-hidden">
        <Button variant="ghost" className="w-full justify-start" onClick={() => setActiveView("chat")}>
          <MessageSquare className="mr-2 h-4 w-4" /> Chats
        </Button>
        <ScrollArea className="flex-grow">
          {conversations.map((conversation) => (
            <Button
              key={conversation.id}
              variant="ghost"
              className="w-full justify-start pl-8 text-sm"
              onClick={() => {
                setActiveConversation(conversation)
                setActiveView("chat")
              }}
            >
              {conversation.title}
            </Button>
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

