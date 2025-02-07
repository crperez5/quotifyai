"use client"

import { useState, useEffect, useRef } from "react"
import { api } from "../../services/api"
import type { Document } from "../../types"
import { Button } from "@/components/ui/button"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Trash2, Upload } from "lucide-react"

export default function DocumentList() {
  const [documents, setDocuments] = useState<Document[]>([])
  const [isUploading, setIsUploading] = useState(false)
  const fileInputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    loadDocuments()
  }, [])

  const loadDocuments = async () => {
    const loadedDocuments = await api.getDocuments()
    setDocuments(loadedDocuments)
  }

  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    if (file) {
      setIsUploading(true)
      try {
        const uploadedDocument = await api.uploadDocument(file)
        setDocuments((prev) => [...prev, uploadedDocument])
      } catch (error) {
        console.error("Failed to upload document:", error)
      } finally {
        setIsUploading(false)
      }
    }
  }

  const handleDeleteDocument = async (id: string) => {
    try {
      await api.deleteDocument(id)
      setDocuments((prev) => prev.filter((doc) => doc.id !== id))
    } catch (error) {
      console.error("Failed to delete document:", error)
    }
  }

  const triggerFileUpload = () => {
    fileInputRef.current?.click()
  }

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Documents</h2>
      <div className="mb-4">
        <Button onClick={triggerFileUpload} disabled={isUploading}>
          <Upload className="mr-2 h-4 w-4" />
          {isUploading ? "Uploading..." : "Upload PDF"}
        </Button>
        <input type="file" ref={fileInputRef} onChange={handleFileUpload} accept=".pdf" style={{ display: "none" }} />
      </div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Upload Date</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {documents.map((document) => (
            <TableRow key={document.id}>
              <TableCell>
                <a
                  href={document.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-blue-500 hover:underline"
                >
                  {document.name}
                </a>
              </TableCell>
              <TableCell>{document.uploadDate.toLocaleString()}</TableCell>
              <TableCell>
                <Button variant="destructive" onClick={() => handleDeleteDocument(document.id)}>
                  <Trash2 className="h-4 w-4" />
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}

