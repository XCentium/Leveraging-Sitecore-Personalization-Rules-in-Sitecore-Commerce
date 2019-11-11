Sitecore Commerce Plugin to leverage Sitecore Personalization Rules Engine.
======================================
Sitecore Personalization is a very popular tool, used widely amongst Marketers and Content Authors. It allows you to deliver targeted content to your visitors. 
For more information on Sitecore Personalization : https://doc.sitecore.com/users/90/sitecore-experience-platform/en/personalizing-components.html

This plugin will allow to extend this feature to Sitecore Commerce Products by extending the commerce variant component with a 'Personalization Id' and filtering Variants in SXA Pipelines based on 'Personalizaton Id' when/if provided in Data Source.

This plugin will allow you for example to unlock Sale Prices/Discounts based on a Sitecore Campaign or Campaign Group.
Or you can use differnet Pricing or Images based on a Sitecore determined Persona, Profile Cards, Pattern Cards or any other visitor behavior.
In other terms you'll be able apply any Sitecore Peronalization Rule on Commerce Products.

Sponsor
=======
This plugin was sponsored and created by Xcentium.


How to Install
==============

Commerce Engine Installation Steps:
============================================

1-Copy the 'XCentium.Commerce.Plugin.CatalogPersonalization' plugin to your XC Solution and add it as a project.  
2-Add it as a dependency to your Sitecore.Commerce.Engine project.  
3-Bootstrap Sitecore Commerce Engine.  
4-Refresh Commerce Cache from Sitecore Content Editor.

SXA Storefront Installation Steps:
============================================

1-Copy the XP 'XCentium.Sitecore.Commerce.XA' project to your XP Solution and add it as a project.  
2-Add it as a dependency to your XP Web Project.  
3-Copy the config files under App_Config/Include/zzz/ to your web project.  
4-Deploy the web project.  
5-Install packages under /Packages folder using Sitecore Package installer.

How to Use
==============
In this usage example, we will create a persona based personalization rule that will unlock a Sale Price for a targeted Product.

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

3-Publish the two created items

Step 3 - Assign personas to seleceted pages:
============================================

1-Create a new Persona or use existing one. In this example we will use existing 'Chris' Persona.  

2-Navigate to a selected page (In this example we use Televisions Category), then go to Experience Editor-> Optimization Tab and assign Values to Personas on this page:  

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/AssignPersona.png)  

Step 4 - Create Personalization Rule for Product List and Product Variants:
===========================================================================

1-Navigate to 'Default Main Category Page Content'-> Personalization Details and create a rule as shown below:  

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/CreatePersonalizationRule.png)

2-Repeat step above for 'Default Main Product Page Content' using 'Product Variants-Chris' as DataSource for personalization rule.

3-Publish updated Items.

Verification:
============

-Navigate to Audio/Headphones. Targeted Product for this personalization displays regular price:  

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/Before.png)

-Click on Televisions Category (Associated with 'Chris' Persona)

-Wait 5 to 10 minutes then Navigate back to Audio/Headphones category. Targeted product for this personalization displays Sale Price:

![alt text](https://github.com/XCentium/Leveraging-Sitecore-Personalization-Rules-in-Sitecore-Commerce/blob/master/Images/After.png)


Conclusion:
==========

In this example we used Persona based Personalization Rule, associated with Sale Price. But with this plugin you can use any Personalization rule and associate it with other changes on the variant (Image, Description...)

This plugin was developped and tested on Sitecore XP 9.1.1 and Sitecore XC 9.1.0
