"use client"

import { useState, useCallback, useEffect } from "react"
import { ConversationService } from "@/services/ConversationService"
import type { Conversation, Message } from "../../types"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import type React from "react"
import { Loader } from "./Loader"
import { formatDate } from "../../lib/formatDate"
import { useWebSocket } from "@/lib/websocket"

interface ChatInterfaceProps {
  setConversations: React.Dispatch<React.SetStateAction<Conversation[]>>
  activeConversation: Conversation
  setActiveConversation: React.Dispatch<React.SetStateAction<Conversation | null>>
}

export default function ChatInterface({ setConversations, activeConversation, setActiveConversation }: ChatInterfaceProps) {
  const [input, setInput] = useState("")
  const [isLoading, setIsLoading] = useState(false)

  const handleOnMessage: ((message: any) => void) = (message) => {
    switch (message.type) {
      case "startAssistantMessage": {
        const newMessage: Message = {
          id: message.messageId,
          content: message.content,
          role: message.role,
          createdAt: message.createdAt
        };

        const updatedConversation = {
          ...activeConversation,
          messages: [...activeConversation.messages, newMessage]
        };
        setActiveConversation(updatedConversation);

        setConversations(prevConversations =>
          prevConversations.map(conv => conv.id === message.conversationId ? ({ ...conv, messages: [...conv.messages, newMessage] }) : conv)
        );
        break;
      }

      case "continueAssistantMessage": {
        const lastMessage = activeConversation.messages.at(-1);
        if (lastMessage) {
          lastMessage.content += message.content;
          const updatedConversation = {
            ...activeConversation,
            messages: [...activeConversation.messages.slice(0, -1), lastMessage]
          };
          setActiveConversation(updatedConversation);
        }

        setConversations(prevConversations => prevConversations.map(conv => {
          if (conv.id === message.conversationId) {
            const lastMessage = conv.messages.at(-1);
            if (lastMessage) {
              lastMessage.content += message.content;
              const updatedConversation = {
                ...conv,
                messages: [...conv.messages.slice(0, -1), lastMessage]
              };
              return updatedConversation;
            }
            return conv;
          }
          return conv;
        }));
        break;
      }

      case "startConversationTitle":
        setConversations(prevConversations =>
          prevConversations.map(conv => conv.id === message.conversationId ? ({ ...conv, title: message.title }) : conv)
        );
        break;

      case "continueConversationTitle":
        setConversations(prevConversations =>
          prevConversations.map(conv => {
            return conv.id === message.conversationId ? ({ ...conv, title: conv.title + message.title }) : conv
          })
        );
        break;

      default:
        break;
    }
  };

  const { joinConversation, leaveConversation } = useWebSocket({
    url: process.env.QUOTIFYAI_WS_URL!,
    onMessage: handleOnMessage,
  });

  useEffect(() => {
    if (activeConversation?.id) {
      joinConversation(activeConversation.id);
    }

    return () => {
      if (activeConversation?.id) {
        leaveConversation(activeConversation.id);
      }
    };
  }, [activeConversation?.id]);

  const handleSendMessage = useCallback(async () => {
    if (!input.trim()) return

    setIsLoading(true);

    const message: Message = {
      id: "",
      content: input,
      role: "user",
    };

    try {
      if (activeConversation.id === '') {
        const createdConversation = await ConversationService.create(message)
        setActiveConversation(createdConversation)
        setConversations(prevConversations =>
          prevConversations.map(conv =>
            conv.id === '' ? createdConversation : conv
          )
        );
      }
      else {
        const createdMessage = await ConversationService.addMessage(activeConversation.id, message)
        setActiveConversation((prev) => ({
          ...prev,
          messages: [...prev.messages, createdMessage]
        }))
        setConversations(prevConversations =>
          prevConversations.map(conv =>
            conv.id === activeConversation.id ? ({ ...conv, messages: [...conv.messages, createdMessage] }) : conv
          )
        );
      }

      setInput("")
    }
    finally {
      setIsLoading(false)
    }
  }, [activeConversation.id, input, setActiveConversation, setConversations])

  return (
    <div className="h-full flex flex-col">
      <ScrollArea className="flex-1 mb-4">
        {activeConversation.messages.map((message: Message) => (
          <div key={message.id} className="mb-4">
            {message.createdAt && <div className="text-xs text-gray-500 mb-1">{formatDate(message.createdAt)}</div>}
            <Card className={message.role === "user" ? "bg-blue-100" : "bg-green-100"}>
              <CardContent className="p-4">
                <p className="font-bold">{message.role === "user" ? "You" : "AI"}</p>
                <p>{message.content}</p>
              </CardContent>
            </Card>
          </div>
        ))}
      </ScrollArea>
      <div className="flex space-x-2">
        <Input
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type your message..."
          onKeyPress={(e) => e.key === "Enter" && handleSendMessage()}
        />
        <Button onClick={handleSendMessage} disabled={isLoading}>
          {isLoading ? <Loader size={20} color="#ffffff" /> : "Send"}
        </Button>
      </div>
    </div>
  )
}



