import sculptprint

sculptprint.start()
sculptprint.new()
sculptprint.import_part(r'\\Client\C$\Users\amarellapudi6\OneDrive - Georgia Institute of Technology\CASS\Solidworks-DFM-PlugIn\SculptPrint\test.STL')
sculptprint.fixture_part(1.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0)
sculptprint.create_cylinder_stock(-0.000010, 0.000008, 0.000000, 26.632273, 8.000000)
sculptprint.define_nocut_zone(0.500000)
sculptprint.import_tool_catalog(r'C:\Program Files\Tucker Innovations\SculptPrint\media\tools\ToolsMetric.xml')
sculptprint.create_volumes(0.012200, 0.000000)
sculptprint.save_as(r'\\Client\C$\Users\amarellapudi6\OneDrive - Georgia Institute of Technology\CASS\Solidworks-DFM-PlugIn\SculptPrint\test.scpr')
