<template>
  <div class="flex flex-row justify-between align-center items-center">
    <h2 class="text-2xl font-medium">Пользователи</h2>
    <button
      type="button"
      class="rounded-lg py-2 px-4 mb-4 bg-green-200 hover:bg-green-300 transition duration-300 font-medium"
      data-bs-toggle="modal"
      data-bs-target="#userModal"
      @click="showModal = true"
    >
      Добавить пользователя
    </button>
  </div>

  <table class="border-separate border-spacing-0 w-full bg-white shadow-lg mb-24">
    <thead>
      <tr class="bg-slate-200 text-slate-800">
        <th
          class="border border-slate-300 p-2"
          v-for="header in headerItemsUser.slice(1)"
          :key="header"
        >
          {{ header }}
        </th>
      </tr>
    </thead>
    <tbody>
      <tr
        v-for="(row, index) in rowsUser"
        :key="index"
        class="hover:bg-slate-100 transition duration-300"
      >
        <td
          v-for="(column, colIndex) in Object.values(row).slice(1)"
          :key="colIndex"
          class="border border-slate-300 p-2 text-left"
        >
          {{ column }}
        </td>
        <td class="border border-slate-300 p-2 text-left">
          <button
            type="button"
            class="bg-blue-200 hover:bg-blue-300 transition duration-300 text-black font-medium py-2 px-4 rounded focus:outline-none focus:shadow-outline mr-2"
            @click="editRow(row)"
          >
            Редактировать
          </button>
          <button
            type="button"
            class="bg-red-200 hover:bg-red-300 transition duration-300 text-gray-700 font-medium py-2 px-4 rounded focus:outline-none focus:shadow-outline"
            @click="deleteRowAlert(row)"
          >
            Удалить
          </button>
        </td>
      </tr>
    </tbody>
  </table>

  <div
    class="modal fade fixed top-0 left-0 w-full h-full bg-gray-900 bg-opacity-50 flex justify-center items-center"
    id="userModal"
    tabindex="-1"
    aria-labelledby="userModalLabel"
    aria-hidden="true"
    v-if="showModal"
  >
    <div class="modal-dialog modal-md bg-white rounded-lg shadow-lg">
      <div class="modal-content p-12">
        <div class="flex justify-between modal-header mb-6">
          <h5 class="modal-title text-lg font-bold text-gray-900" id="userModalLabel">Добавить</h5>
          <button
            type="button"
            class="text-gray-400 hover:text-gray-900 transition duration-300"
            data-bs-dismiss="modal"
            aria-label="Close"
            @click="showModal = false"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>
        <div class="modal-body mb-8">
          <form>
            <div class="mb-4" v-for="(header, index) in headerItemsUser.slice(1)" :key="index">
              <label :for="header" class="block text-gray-700 text-sm font-bold mb-2">{{
                header
              }}</label>
              <input
                v-if="header !== 'Role'"
                type="text"
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                :id="header"
                :placeholder="header"
                v-model="formData[header]"
              />
              <select
                v-else
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                :id="header"
              >
                <option value="admin">admin</option>
                <option value="user">user</option>
              </select>
            </div>
          </form>
        </div>
        <div class="modal-footer flex justify-end">
          <button
            type="button"
            class="bg-green-200 hover:bg-green-300 transition duration-300 text-black font-medium py-2 px-4 rounded focus:outline-none focus:shadow-outline mr-6"
            @click="createRow"
          >
            Сохранить изменения
          </button>
          <button
            type="button"
            class="bg-red-200 hover:bg-red-300 transition duration-300 text-gray-700 font-medium py-2 px-4 rounded focus:outline-none focus:shadow-outline"
            data-bs-dismiss="modal"
            @click="showModal = false"
          >
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, ref, onMounted } from 'vue'
import axios from 'axios'

const showModal = ref(false)

const props = defineProps({
  rowsUser: Array,
  headerItemsUser: Array,
  required: true,
  apiEndpoint: String,
  createRow: Function,
  updateRow: Function,
  deleteRow: Function
})

const formData = ref({})

const rowsUser = ref(props.rowsUser)
const headerItemsUser = ref(props.headerItemsUser)

onMounted(async () => {
  try {
    const response = await axios.get(`http://localhost:5072/api/user`)
    headerItemsUser.push(...Object.keys(response.data[0]))
  } catch (error) {
    console.error(error)
  }
})

const emit = defineEmits(['createRow', 'updateRow', 'deleteRow'])

const createRow = async () => {
  try {
    const formData = new FormData(document.querySelector('#userModal form'))
    const response = await axios.post(`http://localhost:5072/api/user`, formData)
    rowsUser.push(response.data)
    showModal.value = false
  } catch (error) {
    console.error(error)
  }
}

const updateRow = async (row) => {
  try {
    const response = await axios.put(`http://localhost:5072/api/user/${row.UserId}`, {
      Firstname: row.Firstname,
      LastName: row.LastName,
      Patronymic: row.Patronymic,
      Email: row.Email,
      Password: row.Password,
      Role: row.Role
    })
    const index = props.rowsUser.findIndex((r) => r.id === row.UserId)
    props.rowsUser.splice(index, 1, response.data)
  } catch (error) {
    console.error(error)
  }
}

const deleteRow = async (row) => {
  try {
    await axios.delete(`http://localhost:5072/api/user/${row.UserId}`)
    const index = props.rowsUser.findIndex((r) => r.id === row.UserId)
    props.rowsUser.splice(index, 1)
  } catch (error) {
    console.error(error)
  }
}
</script>
