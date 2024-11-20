<template>
  <div class="flex flex-row justify-between align-center items-center">
    <h2 class="text-2xl font-medium">Товары</h2>
    <button
      type="button"
      class="rounded-lg py-2 px-4 mb-4 bg-green-200 hover:bg-green-300 transition duration-300 font-medium"
      @click="openAddModal"
    >
      Добавить товар
    </button>
  </div>

  <table class="border-separate border-spacing-0 w-full bg-white shadow-lg mb-24">
    <thead>
      <tr class="bg-slate-200 text-slate-800">
        <th
          class="border border-slate-300 p-2"
          v-for="header in headerItemsProduct.slice(1)"
          :key="header"
        >
          {{ header }}
        </th>
      </tr>
    </thead>
    <tbody>
      <tr
        v-for="(row, index) in rowsProduct"
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
    v-if="showModal"
  >
    <div class="modal-dialog modal-md bg-white rounded-lg shadow-lg">
      <div class="modal-content p-12">
        <div class="flex justify-between modal-header mb-6">
          <h5 class="modal-title text-lg font-bold text-gray-900">
            {{ editedRow ? 'Редактировать' : 'Добавить' }}
          </h5>
          <button
            type="button"
            class="text-gray-400 hover:text-gray-900 transition duration-300"
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
          <form @submit.prevent="editedRow ? updateRow() : createRow()">
            <div class="mb-4" v-for="(header, index) in headerItemsProduct.slice(1)" :key="index">
              <label :for="header" class="block text-gray-700 text-sm font-bold mb-2">{{
                header
              }}</label>
              <input
                v-if="header !== 'Image'"
                type="text"
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                :id="header"
                :placeholder="header"
                v-model="formData[header]"
              />
              <input
                v-else
                type="file"
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                :id="header"
                @change="handleFileChange($event, header)"
              />
            </div>
            <div class="modal-footer flex justify-between">
              <button
                type="submit"
                class="bg-green-200 hover:bg-green-300 transition duration-300 text-black font-medium py-2 px-4 rounded focus:outline-none focus:shadow-outline mr-6"
              >
                {{ editedRow ? 'Сохранить изменения' : 'Добавить' }}
              </button>
              <button
                type="button"
                class="bg-red-200 hover:bg-red-300 transition duration-300 text-gray-700 font-medium py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                @click="showModal = false"
              >
                Закрыть
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, ref, onMounted, computed } from 'vue'
import axios from 'axios'

const showModal = ref(false)
const editedRow = ref(null)
const newRow = ref(null)

const formData = computed(() => {
  return editedRow.value ? editedRow.value : newRow.value;
});

const props = defineProps({
  rowsProduct: Array,
  headerItemsProduct: Array,
  required: true,
  apiEndpoint: String,
  createRow: Function,
  updateRow: Function,
  deleteRow: Function
})

const rowsProduct = ref(props.rowsProduct)
const headerItemsProduct = ref(props.headerItemsProduct)

onMounted(async () => {
  try {
    const response = await axios.get(`http://localhost:5072/api/product`)
    headerItemsProduct.push(...response.data)
  } catch (error) {
    console.error(error)
  }
})

const emit = defineEmits(['createRow', 'updateRow', 'deleteRow'])

const createRow = async () => {
  try {
    const formData = new FormData();
    formData.append("Name", newRow.value.Name);
    formData.append("Description", newRow.value.Description);
    formData.append("Price", newRow.value.Price);
    const imageInput = document.querySelector('#Image');
    if (imageInput.files.length > 0) {
      const file = imageInput.files[0];
      const imageName = file.name;
      const imagePath = `/sneakers/${imageName}`;
      formData.append("Image", imagePath);
    } else {
      formData.append("Image", newRow.value.Image);
    }
    const response = await axios.post(`http://localhost:5072/api/product`, formData, {
      headers: {
        'Content-Type': 'application/json'
      },
    });
    showModal.value = false;
    editedRow.value = null;
    if (imageInput.files.length > 0) {
      const file = imageInput.files[0];
      const formDataImage = new FormData();
      formDataImage.append("file", file);
      await axios.post(`http://localhost:5072/api/product/SaveFile`, formDataImage, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
      });
    };
    newRow.value = {
      Name: '',
      Description: '',
      Price: '',
      Image: null
    };
  } catch (error) {
    console.error(error);
  }
};

const updateRow = async () => {
  try {
    const formData = new FormData();
    formData.append("ProductId", editedRow.value.ProductId);
    formData.append("Name", editedRow.value.Name);
    formData.append("Description", editedRow.value.Description);
    formData.append("Price", editedRow.value.Price);
    const imageInput = document.querySelector('#Image');
    if (imageInput.files.length > 0) {
      const file = imageInput.files[0];
      const imageName = file.name;
      const imagePath = `/sneakers/${imageName}`;
      formData.append("Image", imagePath);
    } else {
      formData.append("Image", editedRow.value.Image);
    }
    const response = await axios.put(`http://localhost:5072/api/product/`, formData, {
      headers: {
        'Content-Type': 'application/json'
      },
    });
    if (response.data instanceof Object && response.data.id) {
        const index = rowsProduct.value.findIndex((r) => r.id === editedRow.value.id);
        rowsProduct.value.splice(index, 1, response.data);

    } else {
      console.log('Продукт успешно обновлен!');
    }
    showModal.value = false;
    editedRow.value = null;
    if (imageInput.files.length > 0) {
      const file = imageInput.files[0];
      const formDataImage = new FormData();
      formDataImage.append("file", file);
      await axios.post(`http://localhost:5072/api/product/SaveFile`, formDataImage, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
      });
    }
  } catch (error) {
    console.error(error);
  }
};

const openAddModal = () => {
  newRow.value = {
    Name: '',
    Description: '',
    Price: '',
    Image: null
  };
  editedRow.value = null;
  showModal.value = true;
};

const editRow = (row) => {
  editedRow.value = { ...row };
  newRow.value = null;
  showModal.value = true;
};

const deleteRowAlert = (row) => {
  if (confirm(`Вы точно хотите удалить запись ${row.Name}?`)) {
    deleteRow(row)
  }
};

const deleteRow = async (row) => {
  try {
    await axios.delete(`http://localhost:5072/api/product/${row.ProductId}`)
    const index = props.rowsProduct.findIndex((r) => r.id === row.ProductId)
    props.rowsProduct.splice(index, 1)
  } catch (error) {
    console.error(error)
  }
};
</script>