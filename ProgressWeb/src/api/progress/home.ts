import { request } from "@/utils/service"

export interface AlertLine {
  id: number
  lineNo: number
  poNumber: string
  partName: string
  supplierId: number
  supplierName?: string
  requiredDeliveryDate?: string
  leadDays: number
}

export interface SupplierStats {
  supplierId: number
  supplierName?: string
  totalLines: number
  shippedLines: number
  overdueNotShipped: number
  completionRate: number
}

export interface HomeDashboard {
  alerts: AlertLine[]
  supplierStats: SupplierStats[]
}

export function getDashboardApi() {
  return request<{ data: HomeDashboard }>({
    url: "/api/Home/Dashboard",
    method: "get"
  })
}
