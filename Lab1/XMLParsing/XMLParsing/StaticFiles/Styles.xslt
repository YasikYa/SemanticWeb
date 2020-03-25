<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <body>
        <h2>All Vacancies</h2>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>Title</th>
            <th>Date</th>
            <th>City</th>
            <th>Company</th>
            <th>Description</th>
          </tr>
          <xsl:for-each select="ArrayOfVacancy/Vacancy">
            <tr>
              <td>
                <xsl:value-of select="Name"/>
              </td>
              <td>
                <xsl:value-of select="Date"/>
              </td>
              <td>
                <xsl:value-of select="CityName"/>
              </td>
              <td>
                <xsl:value-of select="CompanyName"/>
              </td>
              <td>
                <xsl:value-of select="ShortDescription"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>