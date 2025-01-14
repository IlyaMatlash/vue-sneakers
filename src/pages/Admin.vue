<template>
  <AdminHeader />
  <main>
    <ProductTable
      :rowsProduct="rowsProduct"
      :headerItemsProduct="headerItemsProduct"
      apiEndpoint="http://localhost:5072/api/product"
      @createRow="createRow"
      @updateRow="updateRow"
      @deleteRow="deleteRow"
    />
  </main>
</template>

<script setup>
import ProductTable from '@/components/ProductTable.vue'
import AdminHeader from '@/components/AdminHeader.vue'
import { onMounted, reactive } from 'vue'
import axios from 'axios'

const rowsProduct = reactive([])
const headerItemsProduct = reactive([])
onMounted(async () => {
  try {
    const responseProduct = await axios.get(`http://localhost:5072/api/product`)
    rowsProduct.push(...responseProduct.data)
    if (rowsProduct.length > 0) {
      headerItemsProduct.push(...Object.keys(rowsProduct[0]))
    }
  } catch (error) {
    console.error(error)
  }
})
</script>
