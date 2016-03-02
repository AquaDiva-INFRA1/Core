<?xml version="1.0" encoding="iso-8859-1" ?>
<!--

-->
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:xsd="http://www.w3.org/2001/XMLSchema"
xmlns:asp="xxx"
xmlns:cc1="xxx"
xmlns:exsl="urn:schemas-microsoft-com:xslt"
extension-element-prefixes="exsl"                
>
  <xsl:output method="html" version="4.0" indent="yes"/>
  <xsl:template match="/">
    <script language="JavaScript" type="text/javascript">
      count = 0;

      function setId(listId, lvlId){
        lvl = document.getElementById(lvlId);
        list = document.getElementById(listId);
        lvl.id = "lvl"+count;
        list.id = "list"+count;
        lvl.href = "javascript: expandCollapse('list"+count+"', 'lvl"+count+"');";
        count++;
      }
    </script>
    <html>
      <head>
        <title>Meta Data</title>
      </head>
      <body>
        <div class="box">
          <b><a id="exp" style="margin: 3px" href="javascript: expandAll('list', 'lvl');">+</a></b>/<b><a id="exp" style="margin: 3px" href="javascript: collapseAll('list', 'lvl');">-</a></b>full record
          <ul>
            <xsl:call-template name="lvlFirst" />
          </ul>
          <b><a id="exp" style="margin: 3px" href="javascript: expandAll('list', 'lvl');">+</a></b>/<b><a id="exp" style="margin: 3px" href="javascript: collapseAll('list', 'lvl');">-</a></b>full record
        </div>
      </body>
    </html>
    <script language="JavaScript" type="text/javascript">
      function expandCollapse(listId, lvlId){
        list = document.getElementById(listId);
        lvl = document.getElementById(lvlId);

        if (list.style.display == 'none'){
          list.style.display = 'block';
          lvl.className = 't-icon t-minus';
        }
        else {
          list.style.display='none';
          lvl.className = 't-icon t-plus';
        }
      }

      function expandAll(listPrefix, lvlPrefix){
        for(i=0;i != count;i++){
          list = document.getElementById(listPrefix+i);
          lvl = document.getElementById(lvlPrefix+i);
          list.style.display = 'block';
          lvl.className = 't-icon t-minus';
        }
      }
      
      function collapseAll(listPrefix, lvlPrefix){
        for(i=0;i != count;i++){
          list = document.getElementById(listPrefix+i);
          lvl = document.getElementById(lvlPrefix+i);
          list.style.display = 'none';
          lvl.className = 't-icon t-plus';
        }
      }
    </script>
  </xsl:template>


  <xsl:template name="lvlFirst" match="*">
    <xsl:for-each select="./*">
      <xsl:for-each select="./*">
        <xsl:choose>
          <xsl:when test="text()">
            <li>
              <b>
                <xsl:value-of select="name()" />:
              </b>
              <xsl:value-of select="text()" />
            </li>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="lvlDeeper" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="lvlDeeper" match="*">
    <li>
      <xsl:choose>
        <xsl:when test="count(*)">
          <a id="lvl" class="t-icon t-plus" href="" />
        </xsl:when>
        <xsl:otherwise>
          <a id="lvl" class="t-icon t-plus" style="display: none" href="" />
        </xsl:otherwise>
      </xsl:choose>
      <b><xsl:value-of select="name()" /></b>
      <ul id="list" style="display:none">
        <script language="JavaScript" type="text/javascript">
           setId('list', 'lvl');
        </script>
        <xsl:for-each select="./*">
          <xsl:choose>
            <xsl:when test="text()">
              <li>
                <b>
                  <xsl:value-of select="name()" />:
                </b>
                <xsl:value-of select="text()" />
              </li>
            </xsl:when>
            <xsl:otherwise>
              <xsl:call-template name="lvlDeeper" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </ul>
    </li>
  </xsl:template>              
</xsl:stylesheet>