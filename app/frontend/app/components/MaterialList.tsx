"use client"

import { useState, useEffect } from "react"
import { api } from "../../services/api"
import type { Material } from "../../types"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"

export default function MaterialList() {
  const [materials, setMaterials] = useState<Material[]>([])
  const [newMaterial, setNewMaterial] = useState({ name: "", pricePerSqm: 0 })
  const [editingMaterial, setEditingMaterial] = useState<Material | null>(null)

  useEffect(() => {
    loadMaterials()
  }, [])

  const loadMaterials = async () => {
    const loadedMaterials = await api.getMaterials()
    setMaterials(loadedMaterials)
  }

  const handleCreateMaterial = async () => {
    await api.createMaterial(newMaterial.name, newMaterial.pricePerSqm)
    setNewMaterial({ name: "", pricePerSqm: 0 })
    loadMaterials()
  }

  const handleUpdateMaterial = async () => {
    if (editingMaterial) {
      await api.updateMaterial(editingMaterial.id, editingMaterial.name, editingMaterial.pricePerSqm)
      setEditingMaterial(null)
      loadMaterials()
    }
  }

  const handleDeleteMaterial = async (id: string) => {
    await api.deleteMaterial(id)
    loadMaterials()
  }

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Materials</h2>
      <div className="mb-4 flex space-x-2">
        <Input
          placeholder="Name"
          value={newMaterial.name}
          onChange={(e) => setNewMaterial({ ...newMaterial, name: e.target.value })}
        />
        <Input
          type="number"
          placeholder="Price per sqm (€)"
          value={newMaterial.pricePerSqm}
          onChange={(e) => setNewMaterial({ ...newMaterial, pricePerSqm: Number.parseFloat(e.target.value) })}
        />
        <Button onClick={handleCreateMaterial}>Add Material</Button>
      </div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Price per sqm</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {materials.map((material) => (
            <TableRow key={material.id}>
              <TableCell>
                {editingMaterial?.id === material.id ? (
                  <Input
                    value={editingMaterial.name}
                    onChange={(e) => setEditingMaterial({ ...editingMaterial, name: e.target.value })}
                  />
                ) : (
                  material.name
                )}
              </TableCell>
              <TableCell>
                {editingMaterial?.id === material.id ? (
                  <Input
                    type="number"
                    value={editingMaterial.pricePerSqm}
                    onChange={(e) =>
                      setEditingMaterial({ ...editingMaterial, pricePerSqm: Number.parseFloat(e.target.value) })
                    }
                  />
                ) : (
                  `€${material.pricePerSqm.toFixed(2)}`
                )}
              </TableCell>
              <TableCell>
                {editingMaterial?.id === material.id ? (
                  <Button onClick={handleUpdateMaterial}>Save</Button>
                ) : (
                  <>
                    <Button variant="outline" className="mr-2" onClick={() => setEditingMaterial(material)}>
                      Edit
                    </Button>
                    <Button variant="destructive" onClick={() => handleDeleteMaterial(material.id)}>
                      Delete
                    </Button>
                  </>
                )}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}

