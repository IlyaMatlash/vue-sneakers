<template>
  <AdminHeader />
    <main>
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
  import UserTable from '@/components/UserTable.vue'
  import AdminHeader from '@/components/AdminHeader.vue'
  import { onMounted, reactive } from 'vue'
  import axios from 'axios'
  
  const rowsUser = reactive([])
  const headerItemsUser = reactive([])
  
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
  