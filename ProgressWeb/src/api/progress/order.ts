import { request } from "@/utils/service"

export interface OrderLineDto {
  id: number
  lineNo: number
  poNumber: string
  projectCode: string
  drawingNumber: string
  partName: string
  material?: string
  supplierId: number
  supplierName?: string
  quantity: number
  unit?: string
  receivedQuantity?: number
  requiredDeliveryDate?: string
  latestCraftCode?: string
  vendorUpdatedAt?: string
  vendorEstimatedDeliveryDate?: string
  shippedQuantity?: number
  /** PDF：0 未填写；1 已发货；2 未发货 */
  shippingStatus: number
  supplierNotes?: string
  actualDeliveryDate?: string
  createTime: string
  /** 0 无 1 待供应商 2 返修中 3 已返修发货 */
  repairStatus: number
  repairCreatedAt?: string
  repairStartedAt?: string
  repairShippedAt?: string
}

export interface OrderLineQuery {
  poNumber?: string
  projectCode?: string
  drawingNumber?: string
  partName?: string
  supplierId?: number
  page?: number
  pageSize?: number
}

export function queryOrderLinesApi(body: OrderLineQuery) {
  return request<{ data: { items: OrderLineDto[]; total: number } }>({
    url: "/api/OrderLine/Query",
    method: "post",
    data: body
  })
}

export function getOrderLineApi(id: number) {
  return request<{ data: OrderLineDto }>({
    url: "/api/OrderLine/Get",
    method: "get",
    params: { id }
  })
}

export function createOrderLineApi(data: Record<string, unknown>) {
  return request<{ data: { id: number } }>({
    url: "/api/OrderLine/Create",
    method: "post",
    data
  })
}

export function updateOrderLineApi(data: Record<string, unknown>) {
  return request({
    url: "/api/OrderLine/Update",
    method: "post",
    data
  })
}

export function deleteOrderLineApi(id: number) {
  return request({
    url: `/api/OrderLine/Delete/${id}`,
    method: "post"
  })
}

export function supplierUpdateLineApi(id: number, data: Record<string, unknown>) {
  return request({
    url: `/api/OrderLine/SupplierUpdate/${id}`,
    method: "post",
    data
  })
}

export function createRepairApi(data: { orderLineId: number; description: string }) {
  return request({
    url: "/api/Repair/Create",
    method: "post",
    data
  })
}
