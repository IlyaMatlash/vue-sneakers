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
        <th class="border border-slate-300 p-2">Действия</th>
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

  <!-- Модальное окно с поддержкой скролла -->
  <div
    class="fixed inset-0 z-30 overflow-y-auto"
    v-if="showModal"
  >
    <!-- Overlay -->
    <div 
      class="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
      @click="closeModal"
    ></div>
    
    <!-- Modal container -->
    <div class="flex min-h-full items-center justify-center p-4">
      <div class="relative bg-white w-full max-w-2xl rounded-lg shadow-xl transform transition-all">
        <!-- Modal header - фиксированный -->
        <div class="flex justify-between items-center p-6 border-b border-gray-200">
          <h5 class="text-xl font-bold text-gray-900">
            {{ isEditMode ? 'Редактировать товар' : 'Добавить товар' }}
          </h5>
          <button
            type="button"
            class="text-gray-400 hover:text-gray-600 transition duration-300 p-1"
            @click="closeModal"
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

        <!-- Modal body - -->
        <div class="max-h-[60vh] overflow-y-auto p-6">
          <form @submit.prevent="isEditMode ? updateRow() : createRow()">
            <!-- Отображение ошибок -->
            <div v-if="Object.keys(errors).length > 0" class="mb-4 p-4 bg-red-50 border border-red-200 rounded-md">
              <div v-for="(error, field) in errors" :key="field" class="text-red-600 text-sm">
                <strong>{{ field }}:</strong> {{ error }}
              </div>
            </div>

            <div class="space-y-4">
              <div v-for="(header, index) in headerItemsProduct.slice(1)" :key="index">
                <label :for="header" class="block text-gray-700 text-sm font-medium mb-2">
                  {{ header }}
                  <span v-if="['Name', 'Price', 'Description'].includes(header)" class="text-red-500">*</span>
                </label>
                <input
                  v-if="header !== 'Images'"
                  :type="header === 'Price' ? 'number' : 'text'"
                  :step="header === 'Price' ? '0.01' : undefined"
                  :min="header === 'Price' ? '0' : undefined"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition duration-200"
                  :id="header"
                  :placeholder="header"
                  v-model="formData[header]"
                  :class="{ 'border-red-500': errors[header] }"
                />
                <input
                  v-else
                  type="file"
                  multiple
                  accept="image/*"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-sky-500 transition duration-200"
                  :id="header"
                  @change="handleFileChange($event, header)"
                  :class="{ 'border-red-500': errors.Images }"
                  ref="imageInput"
                />
              </div>
            </div>
          </form>
        </div>

        <!-- Modal footer - фиксированный -->
        <div class="flex justify-end gap-3 p-6 border-t border-gray-200 bg-gray-50">
          <button
            type="button"
            class="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 transition duration-200"
            @click="closeModal"
            :disabled="isLoading"
          >
            Отмена
          </button>
          <button
            type="button"
            @click="isEditMode ? updateRow() : createRow()"
            class="px-4 py-2 bg-sky-500 text-white rounded-md hover:bg-sky-600 focus:outline-none focus:ring-2 focus:ring-sky-500 disabled:opacity-50 disabled:cursor-not-allowed transition duration-200"
            :disabled="isLoading"
          >
            <span v-if="isLoading" class="flex items-center">
              <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Сохранение...
            </span>
            <span v-else>
              {{ isEditMode ? 'Сохранить изменения' : 'Добавить товар' }}
            </span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, ref, onMounted, reactive } from 'vue'
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
const isEditMode = ref(false)
const editingProductId = ref(null)
const errors = ref({})
const isLoading = ref(false)
const imageInput = ref(null)
const selectedFiles = ref(null)

// Реактивные данные формы
const formData = reactive({
  Name: '',
  Description: '',
  Price: '',
  Images: '',
  Brand: '',
  Sizes: '',
  Category: '',
  Season: '',
  Color: '',
  Material: '',
  Features: ''
})

// Валидация формы
const validateForm = (data) => {
  const validationErrors = {}
  
  if (!data.Name?.trim()) {
    validationErrors.Name = 'Название обязательно'
  }
  
  if (!data.Price || parseFloat(data.Price) <= 0) {
    validationErrors.Price = 'Цена должна быть больше 0'
  }
  
  if (!data.Description?.trim()) {
    validationErrors.Description = 'Описание обязательно'
  }
  
  return validationErrors
}

// Валидация изображений
const validateImage = (file) => {
  const maxSize = 5 * 1024 * 1024 // 5MB
  const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg']
  
  if (!allowedTypes.includes(file.type)) {
    return 'Разрешены только JPEG, JPG и PNG'
  }
  
  if (file.size > maxSize) {
    return 'Размер файла не должен превышать 5MB'
  }
  
  return null
}

const emit = defineEmits(['createRow', 'updateRow', 'deleteRow'])

// Очистка формы
const clearForm = () => {
  Object.keys(formData).forEach(key => {
    if (key === 'Price') {
      formData[key] = ''
    } else {
      formData[key] = ''
    }
  })
  selectedFiles.value = null
  errors.value = {}
  
  // Очищаем input файлов
  if (imageInput.value) {
    imageInput.value.value = ''
  }
}

// Закрытие модального окна
const closeModal = () => {
  showModal.value = false
  isEditMode.value = false
  editingProductId.value = null
  clearForm()
}

// Загрузка изображений
const uploadImages = async (files) => {
  const uploadedImages = []
  
  for (let i = 0; i < files.length; i++) {
    const file = files[i]
    const imageError = validateImage(file)
    
    if (imageError) {
      throw new Error(imageError)
    }
    
    try {
      const imageFormData = new FormData()
      imageFormData.append("file", file)
      
      await axios.post(`${props.apiEndpoint}/SaveFile`, imageFormData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      
      const imageName = file.name
      const imagePath = `http://localhost:5173/sneakers/${imageName}`
      uploadedImages.push(imagePath)
    } catch (uploadError) {
      console.error('Error uploading image:', uploadError)
      throw new Error('Ошибка при загрузке изображения')
    }
  }
  
  return uploadedImages
}

// Создание товара
const createRow = async () => {
  try {
    isLoading.value = true
    errors.value = {}
    
    // Валидация формы
    const formErrors = validateForm(formData)
    if (Object.keys(formErrors).length > 0) {
      errors.value = formErrors
      return
    }

    // Подготовка данных для отправки
    const productData = {
      Name: formData.Name,
      Description: formData.Description,
      Price: parseFloat(formData.Price),
      Brand: formData.Brand || '',
      Sizes: formData.Sizes || '',
      Category: formData.Category || '',
      Season: formData.Season || '',
      Color: formData.Color || '',
      Material: formData.Material || '',
      Features: formData.Features || '',
      Images: ''
    }
    
    // Обработка изображений если они есть
    if (selectedFiles.value && selectedFiles.value.length > 0) {
      try {
        const uploadedImages = await uploadImages(selectedFiles.value)
        productData.Images = uploadedImages.join(',')
      } catch (imageError) {
        errors.value.Images = imageError.message
        return
      }
    }
    
    // Отправка данных как JSON
    const response = await axios.post(props.apiEndpoint, productData, {
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    // Обновляем список товаров
    emit('createRow', response.data)
    
    // Закрываем модальное окно
    closeModal()
    
    // Перезагружаем данные
    await refreshData()
    
  } catch (error) {
    console.error('Error creating product:', error)
    
    if (error.response) {
      if (error.response.status === 400) {
        if (error.response.data.errors) {
          // Обработка ошибок валидации от сервера
          const serverErrors = {}
          Object.keys(error.response.data.errors).forEach(field => {
            serverErrors[field] = error.response.data.errors[field].join(', ')
          })
          errors.value = { ...errors.value, ...serverErrors }
        } else {
          errors.value.submit = error.response.data.message || 'Ошибка валидации данных'
        }
      } else if (error.response.status === 415) {
        errors.value.submit = 'Неподдерживаемый тип содержимого'
      } else {
        errors.value.submit = error.response.data.message || 'Ошибка при создании товара'
      }
    } else {
      errors.value.submit = 'Ошибка сети или сервера недоступен'
    }
  } finally {
    isLoading.value = false
  }
}

// Обновление товара
const updateRow = async () => {
  try {
    isLoading.value = true
    errors.value = {}
    
    // Валидация ��ормы
    const formErrors = validateForm(formData)
    if (Object.keys(formErrors).length > 0) {
      errors.value = formErrors
      return
    }

    // Подготовка данных для отправки
    const productData = {
      Name: formData.Name,
      Description: formData.Description,
      Price: parseFloat(formData.Price),
      Brand: formData.Brand || '',
      Sizes: formData.Sizes || '',
      Category: formData.Category || '',
      Season: formData.Season || '',
      Color: formData.Color || '',
      Material: formData.Material || '',
      Features: formData.Features || '',
      Images: formData.Images // Сохраняем существующие изображения
    }
    
    // Обработка новых изображений если они есть
    if (selectedFiles.value && selectedFiles.value.length > 0) {
      try {
        const uploadedImages = await uploadImages(selectedFiles.value)
        productData.Images = uploadedImages.join(',')
      } catch (imageError) {
        errors.value.Images = imageError.message
        return
      }
    }
    
    // Отправка данных как JSON
    const response = await axios.put(`${props.apiEndpoint}/${editingProductId.value}`, productData, {
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    // Обновляем список товаров
    emit('updateRow', response.data)
    
    // Закрываем модальное окно
    closeModal()
    
    // Перезагружаем данные
    await refreshData()
    
  } catch (error) {
    console.error('Error updating product:', error)
    
    if (error.response) {
      if (error.response.status === 400) {
        if (error.response.data.errors) {
          // Обработка ошибок валидации от сервера
          const serverErrors = {}
          Object.keys(error.response.data.errors).forEach(field => {
            serverErrors[field] = error.response.data.errors[field].join(', ')
          })
          errors.value = { ...errors.value, ...serverErrors }
        } else {
          errors.value.submit = error.response.data.message || 'Ошибка валидации данных'
        }
      } else if (error.response.status === 415) {
        errors.value.submit = 'Неподдержива��мый тип содержимого'
      } else {
        errors.value.submit = error.response.data.message || 'Ошибка при обновлении товара'
      }
    } else {
      errors.value.submit = 'Ошибка сети или сервера недоступен'
    }
  } finally {
    isLoading.value = false
  }
}

// Обработка изменения файлов
const handleFileChange = (event, header) => {
  const files = event.target.files
  if (files && files.length > 0) {
    selectedFiles.value = files
  } else {
    selectedFiles.value = null
  }
}

// Открытие модального окна для добавления
const openAddModal = () => {
  clearForm()
  isEditMode.value = false
  editingProductId.value = null
  showModal.value = true
}

// Открытие модального окна для редактирования
const editRow = (row) => {
  clearForm()
  
  // Заполняем форму данными товара
  Object.keys(formData).forEach(key => {
    if (row[key] !== undefined) {
      formData[key] = row[key]
    }
  })
  
  isEditMode.value = true
  editingProductId.value = row.id || row.Id
  showModal.value = true
}

// Подтверждение удаления
const deleteRowAlert = (row) => {
  if (confirm(`Вы точно хотите удалить товар "${row.Name}"?`)) {
    deleteRow(row)
  }
}

// Удаление товара
const deleteRow = async (row) => {
  try {
    const productId = row.id || row.Id
    await axios.delete(`${props.apiEndpoint}/${productId}`)
    emit('deleteRow', productId)
    await refreshData()
  } catch (error) {
    console.error('Error deleting product:', error)
    alert('Ошибка при удалении товара')
  }
}

// Обновление данных
const refreshData = async () => {
  try {
    const response = await axios.get(props.apiEndpoint)
    // Очищаем и обновляем массивы
    props.rowsProduct.splice(0, props.rowsProduct.length, ...response.data)
  } catch (error) {
    console.error('Error refreshing data:', error)
  }
}

onMounted(async () => {
  await refreshData()
})
</script>