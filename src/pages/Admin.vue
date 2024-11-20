<template>
  <main>
    <ProductTable
      :rowsProduct="rowsProduct"
      :headerItemsProduct="headerItemsProduct"
      apiEndpoint="http://localhost:5072/api/product"
      @createRow="createRow"
      @updateRow="updateRow"
      @deleteRow="deleteRow"
    />
    <UserTable
      :rowsUser="rowsUser"
      :headerItemsUser="headerItemsUser"
      apiEndpoint="http://localhost:5072/api/user"
      @createRow="createRow"
      @updateRow="updateRow"
      @deleteRow="deleteRow"
    />
  </main>
</template>

<script setup>
import ProductTable from '@/components/ProductTable.vue'
import UserTable from '@/components/UserTable.vue'
import { onMounted, reactive } from 'vue'
import axios from 'axios'

const rowsProduct = reactive([])
const headerItemsProduct = reactive([])
const rowsUser = reactive([])
const headerItemsUser = reactive([])

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

onMounted(async () => {
  try {
    const responseUser = await axios.get(`http://localhost:5072/api/user`)
    rowsUser.push(...responseUser.data)
    if (rowsUser.length > 0) {
      headerItemsUser.push(...Object.keys(rowsUser[0]))
    }
  } catch (error) {
    console.error(error)
  }
})
</script>
