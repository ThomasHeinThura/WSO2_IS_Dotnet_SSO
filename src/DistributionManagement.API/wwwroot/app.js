// Configuration
const API_BASE = 'http://localhost:5299/api';
let currentUser = null;
let currentRole = null;

// Page Navigation
function goToProducts() {
    document.getElementById('productsPage').classList.remove('hidden');
    document.getElementById('usersPage').classList.add('hidden');
    loadProducts();
}

function goToUsers() {
    document.getElementById('productsPage').classList.add('hidden');
    document.getElementById('usersPage').classList.remove('hidden');
    loadUsers();
}

// ===== LOGIN =====
async function handleLogin(event) {
    event.preventDefault();
    
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorDiv = document.getElementById('loginError');
    
    errorDiv.classList.remove('show');
    errorDiv.innerHTML = '';

    try {
        const response = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            throw new Error('Invalid credentials');
        }

        const data = await response.json();
        
        // Store token
        localStorage.setItem('token', data.accessToken);
        localStorage.setItem('user', JSON.stringify(data.userInfo));
        
        currentUser = data.userInfo;
        currentRole = data.userInfo.roles;

        // Show app
        document.getElementById('loginPage').classList.add('hidden');
        document.getElementById('appPage').classList.remove('hidden');
        
        // Update UI based on role
        updateUIForRole();
        
        // Load products
        loadProducts();
    } catch (error) {
        errorDiv.innerHTML = '❌ ' + error.message;
        errorDiv.classList.add('show');
    }
}

function handleLogout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
    document.getElementById('appPage').classList.add('hidden');
    document.getElementById('loginPage').classList.remove('hidden');
    
    document.getElementById('loginForm').reset();
}

function updateUIForRole() {
    const userDisplay = document.getElementById('userDisplay');
    userDisplay.textContent = `👤 ${currentUser.username} (${currentRole})`;

    // Show/hide admin features
    const isAdmin = currentRole.includes('yks_admin') || currentRole.includes('administrator');
    const isUser = currentRole.includes('yks_user');
    
    document.getElementById('usersBtn').style.display = isAdmin ? 'block' : 'none';
    document.getElementById('addProductBtn').style.display = (isAdmin || isUser) ? 'block' : 'none';
}

// ===== PRODUCTS =====
async function loadProducts() {
    const loading = document.getElementById('productsLoading');
    const table = document.getElementById('productsTable');
    const tbody = document.getElementById('productsBody');
    const errorDiv = document.getElementById('productsError');
    
    loading.style.display = 'block';
    table.classList.add('hidden');
    errorDiv.classList.remove('show');
    tbody.innerHTML = '';

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE}/product`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to load products');
        }

        const products = await response.json();
        
        products.forEach(product => {
            const row = document.createElement('tr');
            const isAdmin = currentRole.includes('yks_admin') || currentRole.includes('administrator');
            const isUser = currentRole.includes('yks_user');
            
            let actionsHTML = '<div class="action-buttons">';
            
            if (isAdmin || isUser) {
                actionsHTML += `<button class="action-btn edit-btn" onclick="openEditProductModal('${product.id}', '${product.name}', '${product.description || ''}', ${product.price}, ${product.stockQuantity}, '${product.category || ''}', '${product.sku || ''}', ${product.isActive})">Edit</button>`;
            }
            
            if (isAdmin) {
                actionsHTML += `<button class="action-btn delete-btn" onclick="deleteProduct('${product.id}')">Delete</button>`;
            }
            
            actionsHTML += '</div>';

            row.innerHTML = `
                <td>${product.name}</td>
                <td>${product.description || '-'}</td>
                <td>$${product.price.toFixed(2)}</td>
                <td>${product.stockQuantity}</td>
                <td>${product.category || '-'}</td>
                <td>${product.sku || '-'}</td>
                <td><span class="status-${product.isActive ? 'active' : 'inactive'}">${product.isActive ? '✓ Active' : '✗ Inactive'}</span></td>
                <td>${actionsHTML}</td>
            `;
            
            tbody.appendChild(row);
        });

        loading.style.display = 'none';
        table.classList.remove('hidden');
    } catch (error) {
        loading.style.display = 'none';
        errorDiv.innerHTML = '❌ ' + error.message;
        errorDiv.classList.add('show');
    }
}

async function handleSaveProduct(event) {
    event.preventDefault();
    
    const editId = document.getElementById('editProductId').value;
    const token = localStorage.getItem('token');
    
    const productData = {
        name: document.getElementById('productName').value,
        description: document.getElementById('productDescription').value,
        price: parseFloat(document.getElementById('productPrice').value),
        stockQuantity: parseInt(document.getElementById('productStock').value),
        category: document.getElementById('productCategory').value,
        sku: document.getElementById('productSku').value,
        isActive: document.getElementById('productActive').checked
    };

    try {
        const method = editId ? 'PUT' : 'POST';
        const url = editId ? `${API_BASE}/product/${editId}` : `${API_BASE}/product`;

        const response = await fetch(url, {
            method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(productData)
        });

        if (!response.ok) {
            throw new Error('Failed to save product');
        }

        closeProductModal();
        loadProducts();
        showSuccessMessage('Product saved successfully!');
    } catch (error) {
        alert('Error: ' + error.message);
    }
}

async function deleteProduct(productId) {
    if (!confirm('Are you sure you want to delete this product?')) {
        return;
    }

    const token = localStorage.getItem('token');

    try {
        const response = await fetch(`${API_BASE}/product/${productId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to delete product');
        }

        loadProducts();
        showSuccessMessage('Product deleted successfully!');
    } catch (error) {
        alert('Error: ' + error.message);
    }
}

function openAddProductModal() {
    document.getElementById('editProductId').value = '';
    document.getElementById('modalTitle').textContent = 'Add New Product';
    document.getElementById('productForm').reset();
    document.getElementById('productModal').classList.remove('hidden');
}

function openEditProductModal(id, name, description, price, stock, category, sku, active) {
    document.getElementById('editProductId').value = id;
    document.getElementById('modalTitle').textContent = 'Edit Product';
    document.getElementById('productName').value = name;
    document.getElementById('productDescription').value = description;
    document.getElementById('productPrice').value = price;
    document.getElementById('productStock').value = stock;
    document.getElementById('productCategory').value = category;
    document.getElementById('productSku').value = sku;
    document.getElementById('productActive').checked = active;
    document.getElementById('productModal').classList.remove('hidden');
}

function closeProductModal() {
    document.getElementById('productModal').classList.add('hidden');
}

// ===== USERS =====
async function loadUsers() {
    const loading = document.getElementById('usersLoading');
    const table = document.getElementById('usersTable');
    const tbody = document.getElementById('usersBody');
    const errorDiv = document.getElementById('usersError');
    
    loading.style.display = 'block';
    table.classList.add('hidden');
    errorDiv.classList.remove('show');
    tbody.innerHTML = '';

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_BASE}/user`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to load users');
        }

        const users = await response.json();
        
        users.forEach(user => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${user.username}</td>
                <td>${user.email}</td>
                <td>${user.role}</td>
            `;
            tbody.appendChild(row);
        });

        loading.style.display = 'none';
        table.classList.remove('hidden');
    } catch (error) {
        loading.style.display = 'none';
        errorDiv.innerHTML = '❌ ' + error.message;
        errorDiv.classList.add('show');
    }
}

function showSuccessMessage(message) {
    const msg = document.createElement('div');
    msg.className = 'success-message show';
    msg.innerHTML = '✓ ' + message;
    document.body.appendChild(msg);
    
    setTimeout(() => msg.remove(), 3000);
}

// Check if already logged in
window.addEventListener('load', () => {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (token && user) {
        currentUser = JSON.parse(user);
        currentRole = currentUser.roles;
        
        document.getElementById('loginPage').classList.add('hidden');
        document.getElementById('appPage').classList.remove('hidden');
        
        updateUIForRole();
        loadProducts();
    }
});
