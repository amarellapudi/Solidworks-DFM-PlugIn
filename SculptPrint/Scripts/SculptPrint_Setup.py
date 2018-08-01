import sculptprint

# Start SculptPrint, import part, fixture part, create cylinder stock, define no cut zone, import tools, and create volumes
sculptprint.start()
sculptprint.new()
sculptprint.import_part(r'\\prism.nas.gatech.edu\rsong8\vlab\desktop\SculptPrint\Experiment Files\test.STL')
sculptprint.fixture_part(1.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0.000000, 0)
sculptprint.create_cylinder_stock(-0.000010, 0.000008, 0.000000, 26.632273, 8.000000)
sculptprint.define_nocut_zone(0.500000)
sculptprint.import_tool_catalog(r'C:\Program Files\Tucker Innovations\SculptPrint\media\tools\ToolsMetric.xml')
sculptprint.create_volumes(0.012200, 0.000000)

# Define and execute first lathe operation
sculptprint.set_active_tool(r'7')
sculptprint.set_active_tool(r'040001')
myturnpass = sculptprint.create_turning_pass()
myturnpass.set_part_margin(0.00000)
myturnpass.set_tool_margin(0.00000)
myturnpass.set_cutting_depth(1.00000)
myturnpass.set_no_cut_r(0.00000)
myturnpass.set_no_cut_z1(0.50000)
myturnpass.set_no_cut_z2(27.43462)
myturnpass.set_bore_hole_depth(0.00000)
myturnpass.set_bore_hole_radius(0.00000)
myturnpass.set_retraction_offset(1.00000)
sculptprint.finalize_turning_pass(myturnpass)

# Improve view and export screenshot
sculptprint.hide_no_cut_plane()
sculptprint.hide_stock_geometry()
sculptprint.hide_cutting_tool()
sculptprint.export_standard_view(r'\\prism.nas.gatech.edu\rsong8\vlab\desktop\SculptPrint\Experiment Files\View_SP.png', r'bottom')

# Save SculptPrint *.scpr file if necessary
#sculptprint.save_as(r'\\Client\C$\Users\amarellapudi6\OneDrive - Georgia Institute of Technology\CASS\Solidworks-DFM-PlugIn\SculptPrint\test.scpr')
