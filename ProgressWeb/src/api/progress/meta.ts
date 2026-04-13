import { request } from "@/utils/service"

export function getSuppliersMetaApi() {
  return request<{ data: { id: number; supplierNumber: string; name: string }[] }>({
    url: "/api/Meta/Suppliers",
    method: "get"
  })
}
