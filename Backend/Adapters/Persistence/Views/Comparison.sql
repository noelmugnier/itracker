select p.name, p.price_value, min(pt.price_value), avg(pt.price_value), max(pt.price_value), count(pt.id) 
from catalog_product_mapping cpm 
join catalog c on c.id = cpm.source_catalog_id
join product p on p.id = cpm.source_product_id
join product pt on pt.id = cpm.target_product_id
group by p.name, p.price_value