<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<title>Розклад занять</title>
				<style>
					body { font-family: Arial, sans-serif; padding: 20px; }
					table { border-collapse: collapse; width: 100%; margin-top: 20px; }
					th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; }
					th { background-color: #4CAF50; color: white; }
					tr:nth-child(even) { background-color: #f2f2f2; }
				</style>
			</head>
			<body>
				<h2>Розклад занять університету</h2>
				<table>
					<tr>
						<th>Факультет</th>
						<th>Кафедра</th>
						<th>Викладач</th>
						<th>Предмет</th>
						<th>Аудиторія</th>
						<th>План (Кр/Год)</th>
						<th>Групи</th>
					</tr>

					<xsl:for-each select="UniversitySchedule/Faculty">
						<xsl:variable name="facultyName" select="@name"/>
						<xsl:for-each select="Department">
							<xsl:variable name="deptName" select="@name"/>
							<xsl:for-each select="Teacher">
								<xsl:variable name="teacherName" select="@name"/>
								<xsl:for-each select="Subject">
									<tr>
										<td>
											<xsl:value-of select="$facultyName"/>
										</td>
										<td>
											<xsl:value-of select="$deptName"/>
										</td>
										<td>
											<xsl:value-of select="$teacherName"/>
										</td>
										<td>
											<xsl:value-of select="@title"/>
										</td>

										<td>
											<xsl:value-of select="@room"/>
											<xsl:if test="@building">
												(к.<xsl:value-of select="@building"/>)
											</xsl:if>
										</td>

										<td>
											<xsl:value-of select="@credits"/> кред. / <xsl:value-of select="@hours"/> год.
										</td>

										<td>
											<xsl:value-of select="Groups"/>
										</td>
									</tr>
								</xsl:for-each>
							</xsl:for-each>
						</xsl:for-each>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>