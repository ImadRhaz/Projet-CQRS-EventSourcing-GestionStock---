import React from 'react';

// Lazy loading components
const Dashboard = React.lazy(() => import('./views/dashboard/Dashboard'));
const Register = React.lazy(() => import('./views/pages/register/Register'));
const Login = React.lazy(() => import('./views/pages/login/Login'));
const ForgotPass = React.lazy(() => import('./views/pages/forgotPassword/ForgotPasswordForm'));
 const Product = React.lazy(() => import('./views/products/Product'));
 //const ProductHistory = React.lazy(() => import('./views/products/ProductHistory'));
const ChangePassword = React.lazy(() => import('./views/ChangePassword/ChangePasswordForm'));
const ResetPassword = React.lazy(() => import('./views/pages/resetPassword/ResetPassword'));
const AddProduct = React.lazy(() => import('./views/products/AddProduct'));
const ProductComponents = React.lazy(() => import('./views/products/ProductComponents'));
const CommandesList = React.lazy(() => import('./views/commandes/CommandesList'));
// const CommandeEnGrosList = React.lazy(() => import('./views/commande-en-gros/CommandeEnGrosList'));
// const ExcelComposents = React.lazy(() => import('./views/products/ExcelComposents')); 
// const FM1Data = React.lazy(() => import('./views/products/Fm1Data')); 




// Define routes
const routes = [
  { path: '/', exact: true, name: 'Home', element: Dashboard },
  { path: '/updatePassword', name: 'UpdatePassword', element: ChangePassword, protected: true },
  { path: '/forgotPassword', name: 'ForgotPass', element: ForgotPass, protected: false },
  { path: '/resetPassword', name: 'ResetPassword', element: ResetPassword },
   { path: '/products', name: 'Product', element: Product },
 { path: '/add-product', name: 'AddProduct', element: AddProduct },
//   { path: '/product-history/:id', name: 'ProductHistory', element: ProductHistory },
 { path: '/product-components/:id', name: 'ProductComponents', element: ProductComponents },
  // { path: '/excelComposents', name: 'ExcelComposents', element: ExcelComposents }, 
  // { path: '/fm1Data', name: 'FM1Data', element: FM1Data }, 
  { path: '/login', name: 'Login', element: Login, protected: false },
  { path: '/dashboard', name: 'Dashboard', element: Dashboard },
  { path: '/register', name: 'Register', element: Register, protected: false },
  { path: '/commandes', name: 'CommandesList', element: CommandesList },
  // { path: '/commande-en-gros', name: 'CommandeEnGrosList', element: CommandeEnGrosList },
];

export default routes;
