-- auth
    -- token
https://localhost:5002/api/authentication/tenant-token

payload:
{
    "userId": 1,
    "userRole": 1,
    "tenantId": 1,
    "locationId": 1,
    "internal": true
}

-- User 
    -- login 
POST https://localhost:5002/api/user/auth/login
Content-Type: application/json

{
  "emailOrPhone": "admin@gmail.com",
  "password": "12341234",
  "rememberMe": false
}

{
    "data": {
        "userId": 1,
        "firstName": "Jerome",
        "lastName": "C",
        "email": "admin@gmail.com",
        "phone": "+91 98705 43210",
        "active": true,
        "tenantId": 1,
        "lastLogin": "2025-09-13T17:19:36.92",
        "rememberMe": false,
        "roles": [
            {
                "roleId": 2,
                "roleName": "",
                "roleDescription": ""
            }
        ],
        "token": null,
        "tokenExpiration": null
    },
    "exception": null
}






    -- reigister
POST https://localhost:5002/api/user/auth/register
Content-Type: application/json

{
    "name": "Jerome C",
    "email": "admin@gmail.com",
    "phone": "+91 98705 43210",
    "password": "12341234",
    "confirmPassword": "12341234",
    "agreeToTerms": true
}

{
    "data": {
        "user": {
            "userId": 1,
            "firstName": "Jerome",
            "lastName": "C",
            "email": "admin@gmail.com",
            "phone": "+91 98705 43210",
            "active": true,
            "tenantId": 1,
            "emailVerified": false,
            "phoneVerified": false,
            "createdAt": "2025-09-13T17:16:52.557",
            "roles": [
                {
                    "roleId": 2,
                    "roleName": "",
                    "roleDescription": ""
                }
            ]
        },
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOjEsImVtYWlsIjoiYWRtaW5AZ21haWwuY29tIiwidGVuYW50SWQiOjEsImV4cCI6MTc1Nzg3MDIxMn0=.signature_placeholder",
        "refreshToken": "1eee397e53ed48d3afdf05d064fc28035f835d4a59fa40f3bc7ab0494d5cec0f",
        "expiresIn": 3600,
        "tokenExpiration": "2025-09-14T17:16:52.5605854Z"
    },
    "exception": null
}



    -- reset password
https://localhost:5002/api/user/auth/reset-password

PAYLOAD:
{
    "userId": "1",
    "newPassword": "NewP789!",
    "confirmPassword": "NewP789!"
}

{
    "data": "Password reset successfully",
    "exception": null
}

    -- logout
https://localhost:5002/api/user/auth/logout
pAYLOAD:
{
    "userId": 1
}

{
    "data": "Logged out successfully",
    "exception": null
}


    -- get user
-- Profile
    -- Get user profile
https://localhost:5002/api/user/auth/profile?userId=1&tenantId=1

{
    "data": {
        "userId": 1,
        "firstName": "Jerome",
        "lastName": "C",
        "fullName": "Jerome C",
        "email": "admin@gmail.com",
        "phone": "+91 98705 43210",
        "active": true,
        "tenantId": 1,
        "emailVerified": false,
        "phoneVerified": false,
        "createdAt": "2025-09-13T17:16:52.557",
        "updatedAt": "2025-09-14T10:54:40.78",
        "lastLogin": "2025-09-14T14:25:26.973",
        "lastLogout": null,
        "profilePicture": "",
        "dateOfBirth": null,
        "gender": "",
        "timezone": "UTC",
        "language": "en",
        "country": "",
        "city": "",
        "state": "",
        "postalCode": "",
        "bio": "",
        "website": "",
        "companyName": "",
        "jobTitle": "",
        "preferredContactMethod": "Email",
        "notificationSettings": "",
        "privacySettings": "",
        "roles": [
            {
                "roleId": 2,
                "roleName": "",
                "roleDescription": ""
            }
        ],
        "addresses": [],
        "preferences": [],
        "statistics": {
            "loginCount": 0,
            "daysSinceLastActivity": 0,
            "profileCompletion": 30
        }
    },
    "exception": null
}


    -- update user profile
https://localhost:5002/api/user/auth/update-profile

{
    "userId": 1,
    "name": "Jerome C",
    "phone": "+91 98765 43710",
    "bio": "Updated bio description",
    "website": "https://newwebsite.com"
}

{
    "data": "Profile updated successfully",
    "exception": null
}


-- product
    -- search product
POST : https://localhost:5002/api/1.0/products/tenants/1/search-products

{
    "page": 1,
    "limit": 10,
    "search": "",
    "category": 1,
    "minPrice": 1.00,
    "maxPrice": 100000.00,
    "rating": "",
    "inStock": "",
    "bestSeller": "",
    "hasOffer": "",
    "sortBy": "price",
    "sortOrder": "asc"
}

{
    "data": {
        "products": [
            {
                "productId": 3,
                "tenantId": 1,
                "productName": "Brinjal",
                "productDescription": "Brinjal description",
                "productCode": "BOD032",
                "fullDescription": "",
                "specification": "",
                "story": "",
                "packQuantity": 1,
                "quantity": 10,
                "total": 10,
                "price": 500.00,
                "category": 1,
                "rating": 6,
                "active": true,
                "trending": 0,
                "userBuyCount": 0,
                "return": 0,
                "created": "2025-09-14T16:13:13.2",
                "modified": "2025-09-14T16:13:13.2",
                "inStock": true,
                "bestSeller": false,
                "deliveryDate": 0,
                "offer": "",
                "orderBy": 0,
                "userId": 1,
                "overview": "",
                "longDescription": "",
                "images": []
            },
            {
                "productId": 1,
                "tenantId": 1,
                "productName": "Apple",
                "productDescription": "Apple description",
                "productCode": "PROD032",
                "fullDescription": "",
                "specification": "",
                "story": "",
                "packQuantity": 1,
                "quantity": 100,
                "total": 100,
                "price": 2999.00,
                "category": 1,
                "rating": 5,
                "active": true,
                "trending": 0,
                "userBuyCount": 0,
                "return": 0,
                "created": "2025-09-14T16:06:49.513",
                "modified": "2025-09-14T16:06:49.513",
                "inStock": true,
                "bestSeller": false,
                "deliveryDate": 0,
                "offer": "",
                "orderBy": 0,
                "userId": 1,
                "overview": "",
                "longDescription": "",
                "images": []
            }
        ],
        "pagination": {
            "page": 1,
            "limit": 10,
            "total": 2,
            "totalPages": 1,
            "hasNext": false,
            "hasPrevious": false
        }
    },
    "exception": null
}

    -- get product
    -- add product 
https://localhost:5002/api/1.0/products/tenants/1/add-product

pl:
{
    "productName": "mni apple",
    "productDescription": "mini APPLE description",
    "productCode": "MAOD032",
    "price": 500,
    "category": 1,
    "quantity": 10,
    "total": 10,
    "userId": 1,
    "fullDescription": "",
    "tenantId": 1,
    "rating": 6,
    "active": true,
    "InStock": 18,
    "originalPrice": 200
}

res:
{
    "data": "Product added successfully",
    "exception": null
}
    -- update product
POST : https://localhost:5002/api/1.0/products/tenants/1/update-product

{
    "productId": 4,
    "productName": "Red Papaya Chilli",
    "productDescription": "Red Papaya",
    "productCode": "PAYGVDFAOD032",
    "price": 500,
    "category": 1,
    "quantity": 10,
    "total": 10,
    "userId": 1,
    "fullDescription": "",
    "tenantId": 1,
    "rating": 4,
    "active": true,
    "InStock": 1,
    "originalPrice": 200
}


{
    "data": "Product updated successfully",
    "exception": null
}
    -- delete product

    delete : https://localhost:5002/api/1.0/products/tenants/1/5

    {
    "data": "Product deleted successfully",
    "exception": null
}

-- image
    -- add image
    -- get images
-- category
    -- add-category
    POST: https://localhost:5002/api/1.0/products/tenantId/1/add-category

    PL:
    {
    "categoryName": "Updated Category Name",
    "description": "Updated category description",
    "active": true,
    "parentCategoryId": null,
    "orderBy": 2,
    "icon": "updated-icon.png",
    "hasSubMenu": true,
    "link": "/categories/updated-category"
    }

    RS:
    {
    "data": {
        "categoryId": 2,
        "category": "Seed",
        "active": true,
        "parentId": null,
        "description": "seed category description",
        "orderBy": 2,
        "icon": "updated-icon.png",
        "subMenu": true,
        "link": "/categories/updated-category",
        "created": "2025-09-16T01:46:36.4450041Z",
        "tenantId": 1
    },
    "exception": null
}

    -- get category
    -- update category

    PUT : https://localhost:5002/api/1.0/products/tenantId/1/update-category/1
    PL:
    {
    "categoryId": 1,
    "categoryName": "Updated Category Name",
    "description": "Updated category description",
    "active": true,
    "parentCategoryId": null,
    "orderBy": 2,
    "icon": "updated-icon.png",
    "hasSubMenu": true,
    "link": "/categories/updated-category"
}

    RS:
    {
    "data": "Category updated successfully",
    "exception": null
}
    -- mernu master
    GET : https://localhost:5002/api/1.0/products/menu/master?tenantId=1
    RS:
    {
    "data": {
        "menuMaster": [
            {
                "menuId": 1,
                "menuName": "Shop",
                "orderBy": 1,
                "active": true,
                "image": "",
                "subMenu": true,
                "tenantId": 1,
                "created": "2025-09-16T01:56:42.277",
                "modified": "2025-09-16T01:56:42.277",
                "category": []
            },
            {
                "menuId": 2,
                "menuName": "Categories",
                "orderBy": 2,
                "active": true,
                "image": "",
                "subMenu": true,
                "tenantId": 1,
                "created": "2025-09-16T01:56:42.277",
                "modified": "2025-09-16T01:56:42.277",
                "category": [
                    {
                        "categoryId": 3,
                        "category": "Electronics",
                        "active": true,
                        "orderBy": 1,
                        "icon": "fa-laptop",
                        "description": "Electronic devices and accessories"
                    },
                    {
                        "categoryId": 4,
                        "category": "Clothing",
                        "active": true,
                        "orderBy": 2,
                        "icon": "fa-tshirt",
                        "description": "Fashion and apparel"
                    },
                    {
                        "categoryId": 2,
                        "category": "Seed",
                        "active": true,
                        "orderBy": 2,
                        "icon": "updated-icon.png",
                        "description": "seed category description"
                    },
                    {
                        "categoryId": 1,
                        "category": "Updated Category Name",
                        "active": true,
                        "orderBy": 2,
                        "icon": "updated-icon.png",
                        "description": "Updated category description"
                    },
                    {
                        "categoryId": 5,
                        "category": "Home & Garden",
                        "active": true,
                        "orderBy": 3,
                        "icon": "fa-home",
                        "description": "Home improvement and garden supplies"
                    },
                    {
                        "categoryId": 6,
                        "category": "Sports & Outdoors",
                        "active": true,
                        "orderBy": 4,
                        "icon": "fa-football-ball",
                        "description": "Sports equipment and outdoor gear"
                    },
                    {
                        "categoryId": 7,
                        "category": "Books & Media",
                        "active": true,
                        "orderBy": 5,
                        "icon": "fa-book",
                        "description": "Books, movies, and digital media"
                    },
                    {
                        "categoryId": 8,
                        "category": "Health & Beauty",
                        "active": true,
                        "orderBy": 6,
                        "icon": "fa-heart",
                        "description": "Health, wellness, and beauty products"
                    },
                    {
                        "categoryId": 9,
                        "category": "Food & Beverages",
                        "active": true,
                        "orderBy": 7,
                        "icon": "fa-utensils",
                        "description": "Food, drinks, and groceries"
                    },
                    {
                        "categoryId": 10,
                        "category": "Automotive",
                        "active": true,
                        "orderBy": 8,
                        "icon": "fa-car",
                        "description": "Car parts and automotive accessories"
                    }
                ]
            },
            {
                "menuId": 3,
                "menuName": "Deals",
                "orderBy": 3,
                "active": true,
                "image": "",
                "subMenu": false,
                "tenantId": 1,
                "created": "2025-09-16T01:56:42.277",
                "modified": "2025-09-16T01:56:42.277",
                "category": []
            },
            {
                "menuId": 4,
                "menuName": "New Arrivals",
                "orderBy": 4,
                "active": true,
                "image": "",
                "subMenu": false,
                "tenantId": 1,
                "created": "2025-09-16T01:56:42.277",
                "modified": "2025-09-16T01:56:42.277",
                "category": []
            },
            {
                "menuId": 5,
                "menuName": "Best Sellers",
                "orderBy": 5,
                "active": true,
                "image": "",
                "subMenu": false,
                "tenantId": 1,
                "created": "2025-09-16T01:56:42.277",
                "modified": "2025-09-16T01:56:42.277",
                "category": []
            }
        ]
    },
    "exception": null
}

https://localhost:5002/api/1.0/products/tenants/10/upload

rs:
{
    "id": 1,
    "guid": "00000000-0000-0000-0000-000000000000",
    "imageName": "julius silvert - Copy (12).jpg",
    "contentType": "image/jpeg",
    "fileSize": 1518130,
    "createdAt": "2025-09-25T02:10:46.078537Z",
    "imageUrl": "https://localhost:5002/api/1.0/products/1",
    "thumbnailUrl": "https://localhost:5002/api/1.0/products/1/thumbnail"
}


https://localhost:5002/api/1.0/products/1


https://localhost:5002/api/1.0/products/1/thumbnail

-- cart
    -- add to cart
    -- update cart
    -- clear cart
    -- get cart
-- order
    -- get orders - admin
    -- get order by id or user
    -- order cancel
    -- order status update
    -- make order
-- wish list
    -- add to wish list
    -- update wish list
    -- clear wish list
    -- get wish list