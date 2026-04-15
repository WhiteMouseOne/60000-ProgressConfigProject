import { request } from "@/utils/service"

/** 分页查询（与用户 GetUserData 一致：res.data = { total, dataList }） */
export function querySupplierAdminApi(data:{
  page:number
  size:number
  supplierNumber?:string
  name?:string
  enable?:number|null
}){
  return request({
    url:"/api/SupplierAdmin/Query",
    method:"post",
    data
  });
}

export function createSupplierAdminApi(data: { supplierNumber: string; name: string; enable?: number }) {
  return request({
    url: "/api/SupplierAdmin/Create",
    method: "post",
    data
  })
}

export function getSupplierListAdminApi() {
  return request({
    url: "/api/SupplierAdmin/List",
    method: "get"
  })
}

/** 与 SupplierAdminController.Delete 一致：HttpPost("{id:int}") */
export function deleteSupplierAdminApi(id: number) {
  return request({
    url: `/api/SupplierAdmin/Delete/${id}`,
    method: "post"
  })
}

/** 后端 SupplierBatchDeleteDto 属性 Ids，JSON 为 ids */
export function batchDeleteSupplierAdminApi(data: { ids: number[] }) {
  return request({
    url: "/api/SupplierAdmin/BatchDelete",
    method: "post",
    data
  })
}

export function updateSupplierAdminApi(id:number,data:Record<string,unknown>){
  return request({
    url:`/api/SupplierAdmin/Update/${id}`,
    method:"post",
    data
  });
}

export function getCraftListAdminApi() {
  return request({
    url: "/api/CraftAdmin/List",
    method: "get"
  })
}

export function queryCraftAdminApi(data: { page: number; pageSize: number; name?: string }) {
  return request({
    url: "/api/CraftAdmin/Query",
    method: "post",
    data
  })
}

export function createCraftAdminApi(data: { code: number; name: string; recipeBody?: string | null }) {
  return request({
    url: "/api/CraftAdmin/Create",
    method: "post",
    data
  })
}

export function updateCraftAdminApi(id: number, data: { code: number; name: string; recipeBody?: string | null }) {
  return request({
    url: `/api/CraftAdmin/Update/${id}`,
    method: "post",
    data
  })
}

export function deleteCraftAdminApi(id: number) {
  return request({
    url: `/api/CraftAdmin/Delete/${id}`,
    method: "post"
  })
}

export function getAlertConfigApi() {
  return request({
    url: "/api/AlertConfig/Get",
    method: "get"
  })
}

export function saveAlertConfigApi(data: { id: number; leadDays: number; enabled: boolean }) {
  return request({
    url: "/api/AlertConfig/Save",
    method: "post",
    data
  })
}

export function queryCraftRecipeAdminApi(data:{page:number;pageSize:number;name?:string}){
  return request({
    url:"/api/CraftRecipeAdmin/Query",
    method:"post",
    data
  })
}

export function createCraftRecipeAdminApi(data:{code:number,name:string}){
  return request({
    url:"/api/CraftRecipeAdmin/Create",
    method:"post",
    data
  })
}

export function updateCraftRecipeAdminApi(id:number,data:{code:number;name:string}){
  return request({
    url:`/api/CraftRecipeAdmin/Update/${id}`,
    method:"post",
    data
  })
}


export function deleteCraftRecipeAdminApi(id: number) {
  return request({
    url: `/api/CraftRecipeAdmin/Delete/${id}`,
    method: "post"
  })
}
