Sitecore Commerce Plugin to leverage Sitecore Personalization Rules Engine.
======================================
Sitecore Personalization is a very popular tool, used widely amongst Marketers and Content Authors. It allows you to deliver targeted content to your visitors. 
For more information on Sitecore Personalization : https://doc.sitecore.com/users/90/sitecore-experience-platform/en/personalizing-components.html

This plugin will allow to extend this feature to Sitecore Commerce Products by extending the commerce variant component with a 'Personalization Id' and filtering Variants in SXA Pipelines based on 'Personalizaton Id' when/if provided in Data Source.

Sponsor
=======
This plugin was sponsored and created by Xcentium.


How to Install
==============

XC (Experience Commerce) Installation Steps:
============================================

1-Copy the 'XCentium.Commerce.Plugin.CatalogPersonalization' plugin to your XC Solution and add it as a project.  
2-Add it as a dependency to your Sitecore.Commerce.Engine project.  
3-Bootstrap Sitecore Commerce Engine.  
4-Refresh Commerce Cache from Sitecore Content Editor.

XP (Experience Platform) Installation Steps:
============================================

1-Copy the XP 'XCentium.Sitecore.Commerce.XA' project to your XP Solution and add it as a project.  
2-Add it as a dependency to your XP Web Project.  
3-Copy the config file under App_Config/Include/zzz/ to your web project.  
4-Deploy the web project.  
5-Install XcProductData-1.0.0.zip package. This overrides ProductList and ProductVariants DataSource Template to extend with 'Personalization Id'.

How to Use
==============
In this usage example, we will create a persona based personalization rule that will unlock a Sale Price for targeted Product.

Step 1 - Create a new variant with personalization for the targeted product:
============================================================================
1-Create a new Entity Version at the SellableItem level.  

2-Create a new Variant and copy values from existing Variant or from Parent Sellable Item.  

3-Add a Personalization ID 'Chris_Persona' in Variant Peronalization Component: 

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/VariationPersonalization.png)  

4-Create or use existing Price Card to apply a Sale Price for this Variant.  

5-Approve the new Entity Version.

Step 2 - Create ProductList and ProductVariants DataSources for the personalization:
===================================================================================

1-Duplicate Product List Data Source and assign Personalization ID 'Chris_Persona' :

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/ProductListDuplicate.png) 

2-Duplicate Product List Data Source and assign Personalization ID 'Chris_Persona' :

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/ProductVariantsDuplicate.png) 
