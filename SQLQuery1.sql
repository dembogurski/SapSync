 
  
   
   SELECT o.AbsEntry, i.ItemCode,i.BatchNum, it.U_NOMBRE_COM as ItemName, i.U_design FROM  OIBT i inner join  OBTN o on i.ItemCode = o.ItemCode and  o.DistNumber = i.BatchNum    
	inner join OITM it  on i.ItemCode = it.ItemCode and it.ItemCode = 'S0007' and i.BatchNum = '11115716'




   SELECT o.AbsEntry, i.ItemCode,i.BatchNum, it.U_NOMBRE_COM as ItemName, i.U_design FROM  OIBT i inner join  OBTN o on i.ItemCode = o.ItemCode and  o.DistNumber = i.BatchNum    
	inner join OITM it  on i.ItemCode = it.ItemCode and it.ItemCode = 'C0080' and i.BatchNum = '6950114' inner join PDN1 p on i.BaseEntry = p.DocEntry and i.BaseLinNum = p.LineNum  

	select * from obtn 


SELECT Price, o.U_desc1 as Descuento  FROM OIBT o, ITM1 p where o.ItemCode = p.ItemCode and o.ItemCode = 'H0007' and p.PriceList = 1 and o.BatchNum = '7639813' and WhsCode LIKE '02' 


select * from opdn   where         WhsCode = '02' and Quantity > 10 

  
 SELECT cast(round(Price,2) as numeric(20,0)) as Precio FROM ITM1 WHERE ItemCode = 'H0007' AND PriceList = 1;

 SELECT * FROM ITM1 WHERE ItemCode = 'H0007' AND PriceList = 1;

 
SELECT  AvgPrice  FROM PDN1 a INNER JOIN OITM b ON a.ItemCode = b.ItemCode AND a.ItemCode = 'B0016' 

 SELECT TOP 1 AvgPrice  FROM PDN1 a INNER JOIN OITW b on a.ItemCode = b.ItemCode and a.ItemCode = 'B0016' 


 SELECT ItemCode,cast(round(Quantity - ISNULL(IsCommited,0),2) as numeric(20,2)) as Stock FROM OIBT o where  o.BatchNum = '51616' and WhsCode LIKE '00' ;

 select U_img,U_F1,U_F2,U_F3,U_factor_precio,U_ubic,U_color_comercial,U_color_comb,U_color_cod_fabric,U_prov_mar,U_bag,U_umc,U_ancho,U_gramaje_m,U_design,U_rec,U_estado_venta,U_desc1,U_desc2,U_desc3,U_desc4,U_desc5,U_desc6,U_desc7
 from OIBT where ItemCode = 'B0016' and BatchNum = '51616'

 SELECT * FROM OWHS 
 
 SELECT * FROM OIQI  

 SELECT * FROM NNM1  
 
 SELECT * FROM OIBT WHERE BatchNum = 'lOTE!'

 select * from    ( SELECT o.DocNum as NroDoc,'' AS CardName ,CONVERT(VARCHAR(10), o.DocDueDate, 105) AS Fecha,i.ItemCode as Codigo,it.ItemName as Articulo,i.BatchNum as Lote,i.WhsCode as Suc, 
 cast(round(Quantity - ISNULL(i.IsCommited,0),2) as numeric(20,2)) as Stock, AvgPrice as PrecioCosto,Price,
 p.PriceList,     CONVERT(DECIMAL(10,2),U_desc1) as U_desc1,U_desc2,U_desc3,U_desc4,U_desc5,U_desc6,U_desc7,i.U_estado_venta AS EstadoVenta FROM OIQI o,OIBT i, OITM it, ITM1 p
 WHERE o.DocEntry = i.BaseEntry and Quantity > 0 and i.ItemCode = it.ItemCode and i.ItemCode = p.ItemCode  and it.FrozenFor = 'N'  
 and i.U_F1 = 0 and i.U_F2   = 0 and i.U_F3  = 0 and   i.ItemCode like '%' and i.U_estado_venta = 'Normal'  and i.BatchNum in('LOTE!')  ) as src
 PIVOT ( AVG(Price) FOR PriceList in ([1],[2],[3],[4],[5],[6],[7])) as Pvt
 	 
 select * from OITW where ItemCode = 'H0007'


select distinct 'FV' as Tipo, o.DocEntry,InstlmntID,CONVERT(VARCHAR(10), DocDate, 103) DocDate,CONVERT(VARCHAR(10), DueDate, 103) DueDate,DATEDIFF(day,DueDate,GETDATE()) AS DiasAtraso,  InsTotal, InsTotalFC,Paid ,i.PaidFC as PaidFC,FolioNum,DocCur 
 ,i.Status  from oinv o, inv6 i  where o.DocEntry = i.DocEntry and o.GroupNum != -1 and    o.CardCode =  'C106525' and i.Status != 'C'  



select * from OIBT  where BatchNum = '20145115'
 
 SELECT ItemCode,cast(round(Quantity - ISNULL(IsCommited,0),2) as numeric(20,2)) as Stock, U_gramaje,U_ancho,U_factor_precio,U_img,c.Name as U_color_comercial,
                U_F1,U_F2,U_F3,Status,WhsCode AS Suc,U_img as Img,U_tara,o.U_desc1 as Descuento 
                FROM OIBT o LEFT JOIN [@EXX_COLOR_COMERCIAL] c ON o.U_color_comercial = c.Code where  o.BatchNum = '20145115' and WhsCode LIKE '02' ;

 SELECT  ItemName as Descrip,U_NOMBRE_COM as NombreComercial, InvntryUom as UM,cast(round(OnHand,2) as numeric(20,0)) as TotalGlobal FROM OITM WHERE ItemCode = 'S0007'

select avgprice from OITM WHERE ItemCode = 'H0007'
 

select * from [@EXX_COLOR_COMERCIAL] where Name = 'ROSA BEBE'


select * from OITM  where ItemCode = 'C0001'


select * from ITM1  where ItemCode = 'B0001'


select * from ITM1 where Price < 1 and PriceList < 8

select * from [@EXX_COLOR_COMERCIAL] where Name = 'LILA OBISPO'


 SELECT  LicTradNum ,count(*) FROM OCRD group by LicTradNum having count(LicTradNum) > 1 






SELECT * FROM [@EXX_COLOR_COMERCIAL] where Name like  ''


delete from [@EXX_COLOR_COMERCIAL]
 

select * from ocrc order by CardName asc


select WhsCode as Suc,ItemCode as CodigoArticulo,BatchNum as CodigoLote,ItemName as Descrip,U_ancho,U_color_comb,Quantity as Stock from oibt  
where WhsCode   in ('02' ) and Quantity > 5 
order by  WhsCode asc 


select CONCAT('|',WhsCode,'|') from OWHS   


SELECT PriceList,Price from  ITM1 WHERE ItemCode = 'C0015' and PriceList < 8

SELECT o.AbsEntry, i.ItemCode,i.BatchNum  FROM  OIBT i inner join  OBTN o on i.ItemCode = o.ItemCode and  o.DistNumber = i.BatchNum  inner join OITM it  on i.ItemCode = it.ItemCode and it.ItemCode = 'H0038' and i.BatchNum = '20234015'

select top 1 CONVERT(VARCHAR(10), o.DocDate, 103) DocDate  from ORCT o, RCT2 r where o.DocNum = r.DocNum  and r.DocEntry = 67 AND InstId = 1 order by r.DocNum desc 


SELECT * FROM oibt where BatchNum = '6950114'

select *   from OITM  where frozenFor = 'N' and U_EST_VENT is null

 
select BatchNum, Quantity,WhsCode from oibt  where   BatchNum = '9178814' 

  

 select  Code,Name,IsNull(U_rgb,'')  as U_rgb,IsNull(U_cod_interno,'')  as U_cod_interno,U_estado from  [@EXX_COLOR_COMERCIAL] where U_estado = 'Activo' or U_estado is null


 SELECT  AvgPrice,cast(round(Quantity - isNull(i.IsCommited,0),2) as numeric(20,2)) as StockReal FROM OITM o, OIBT i WHERE  o.ItemCode = i.ItemCode and i.ItemCode = 'B0006' and i.BatchNum = '1328411' and WhsCode = '02'


 select BatchNum as Lote, U_img as Img, cast(round(Quantity - isNull(IsCommited,0),2) as numeric(20,2)) as StockReal from OIBT WHERE BatchNum = '1328411'


 select count(*) from OIBT 



 SELECT top 120 BatchNum, Quantity FROM   OIBT i where   WhsCode = '02' and BatchNum = '3956914'


 select * from [@EXX_COLOR_COMERCIAL]

 select  Code,Name,IsNull(U_rgb,'')  as U_rgb,IsNull(U_cod_interno,'')  as U_cod_interno,U_estado from  [@EXX_COLOR_COMERCIAL] where U_estado = 'Activo' or U_estado is null