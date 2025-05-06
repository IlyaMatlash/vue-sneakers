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
    class="fixed inset-0 flex justify-center items-center z-30"
    v-if="showModal"
  >
  <!-- <div @click="showModal = false" class="fixed inset-0 bg-black opacity-70">  
  </div> -->
    <div class="modal-dialog modal-md bg-white w-full max-w-[50rem] max-h-[90vh] rounded-lg shadow-lg">
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

const props = defineProps({
  rowsProduct: {
    type: Array,
    required: true
  },
  headerItemsProduct: {
    type: Array,
    required: true
  },
  apiEndpoint: {
    type: String,
    required: true
  }
})
const showModal = ref(false)
const editedRow = ref(null)
const newRow = ref(null)
const errors = ref({})
const isLoading = ref(false)

// Валидация формы
const validateForm = (data) => {
  const errors = {}
  
  if (!data.Name?.trim()) {
    errors.Name = 'Название обязательно'
  }
  
  if (!data.Price || data.Price <= 0) {
    errors.Price = 'Цена должна быть больше 0'
  }
  
  if (!data.Description?.trim()) {
    errors.Description = 'Описание обязательно'
  }
  
  return errors
}

// Валидация изображений
const validateImage = (file) => {
  const maxSize = 5 * 1024 * 1024 // 5MB
  const allowedTypes = ['image/jpeg', 'image/png']
  
  if (!allowedTypes.includes(file.type)) {
    return 'Разрешены только JPEG и PNG'
  }
  
  if (file.size > maxSize) {
    return 'Размер файла не должен превышать 5MB'
  }
  
  return null
}
const formData = computed(() => {
  return editedRow.value ? editedRow.value : newRow.value;
});


const rowsProduct = ref(props.rowsProduct)
const headerItemsProduct = ref(props.headerItemsProduct)

onMounted(async () => {
  try {
    const response = await axios.get(`${props.apiEndpoint}`)
    headerItemsProduct.push(...response.data)
  } catch (error) {
    console.error(error)
  }
})

const emit = defineEmits(['createRow', 'updateRow', 'deleteRow'])

const createRow = async () => {
  try {
    isLoading.value = true
    
    // Валидация формы
    const formErrors = validateForm(newRow.value)
    if (Object.keys(formErrors).length > 0) {
      errors.value = formErrors
      return
    }

    const formData = new FormData();
    formData.append("Name", newRow.value.Name);
    formData.append("Description", newRow.value.Description);
    formData.append("Price", newRow.value.Price);
    formData.append("Brand", newRow.value.Brand);
    
    // Обработка множественных размеров
    if (Array.isArray(newRow.value.Sizes)) {
      formData.append("Sizes", newRow.value.Sizes.join(','));
    } else {
      formData.append("Sizes", newRow.value.Sizes || '');
    }
    
    // Обработка множественных изображений
    const imageInput = document.querySelector('#Images');
    if (imageInput.files.length > 0) {
      const images = [];
      for (let i = 0; i < imageInput.files.length; i++) {
        const file = imageInput.files[i];
        const imageName = file.name;
        const imagePath = `http://localhost:5173/sneakers/${imageName}`;
        images.push(imagePath);
        const imageError = validateImage(file)
        
        if (imageError) {
          errors.value.image = imageError
          return
        }
        
        // Загрузка каждого изображения
        const imageFormData = new FormData();
        imageFormData.append("file", file);
        await axios.post(`${props.apiEndpoint}/SaveFile`, imageFormData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });
      }
      formData.append("Images", images.join(','));
    }
    formData.append("Category", newRow.value.Category);
    formData.append("Season", newRow.value.Season);
    formData.append("Color", newRow.value.Color);
    formData.append("Material", newRow.value.Material);
    formData.append("Features", newRow.value.Features);
    
    const response = await axios.post(props.apiEndpoint, formData);
    showModal.value = false;
    editedRow.value = null;
  } catch (error) {
    console.error('Error creating product:', error)
    errors.value.submit = 'Ошибка при создании товара'
  } finally {
    isLoading.value = false
  }
};

const openAddModal = () => {
  newRow.value = {
    Name: '',
    Description: '',
    Price: '',
    Images: null,
    Brands: '',
    Sizes: '',
    Seasons: '',
    Colors: '',
    Materials: '',
    Features: ''
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
  if (!confirm(`Вы уверены что хотите удалить ${row.Name}?`)) {
    return
  }
  try {
    await axios.delete(`${props.apiEndpoint}/${row.id}`)
    emit('deleteRow', row.id)
  } catch (error) {
    alert('Ошибка при удалении записи')
    console.error(error)
  }
}
</script>