export const parseImagePaths = (images) => {
    if (!images) {
      return ['/sneakers/default-image.jpg']; // Изображение по умолчанию
    }
    
    if (Array.isArray(images)) {
      return images.length > 0 ? images : ['/sneakers/default-image.jpg'];
    }
    
    if (typeof images === 'string') {
      return images.split(',').map(path => path.trim()).filter(Boolean);
    }
    
    return ['/sneakers/default-image.jpg'];
  };
  
  /**
   * Получает первое изображение из списка
   * @param {string|Array} images - Строка с путями, разделенными запятыми, или массив путей
   * @returns {string} - Путь к первому изображению
   */
  export const getFirstImage = (images) => {
    const imageArray = parseImagePaths(images);
    return imageArray[0];
  };