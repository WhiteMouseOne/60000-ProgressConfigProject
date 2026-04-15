import { request } from "@/utils/service"

export function getSuppliersMetaApi() {
  return request<{ data: { id: number; supplierNumber: string; name: string }[] }>({
    url: "/api/Meta/Suppliers",
    method: "get"
  })
}

export interface CraftRecipeLiteDto {
  id: number
  code: number
  name: string
}

export interface CraftInRecipeStepDto {
  craftId: number
  craftCode: number
  craftName: string
  stepOrder: number
}

export function getCraftRecipesMetaApi() {
  return request<{ data: CraftRecipeLiteDto[] }>({
    url: "/api/Meta/CraftRecipes",
    method: "get"
  })
}

export function getCraftRecipeCraftsMetaApi(craftRecipeId: number) {
  return request<{ data: CraftInRecipeStepDto[] }>({
    url: "/api/Meta/CraftRecipeCrafts",
    method: "get",
    params: { craftRecipeId }
  })
}
